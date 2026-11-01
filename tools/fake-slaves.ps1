# fake-slaves.ps1  --  8 fake Modbus-TCP power supplies for ChipAgingCheck testing
# ASCII only (PowerShell 5.1 reads this file as ANSI).
#
# Usage (run from the solution folder):
#   powershell -NoProfile -ExecutionPolicy Bypass -File tools\fake-slaves.ps1
#   powershell -NoProfile -ExecutionPolicy Bypass -File tools\fake-slaves.ps1 -Port 1502 -SlaveCount 8
#
# Listens on 127.0.0.1 .. 127.0.0.8, one slave per address, same port.
# Register map (holding registers, FC03 read / FC16 write), same as the customer appendix:
#   0        furnace temperature, value/10 = degC   (only slave #1 really matters)
#   1..64    channel current,      value/10 = mA
#   65..128  channel voltage,      value/10 = V
#   129      power supply state (0 standby / 1 outputting / 2 fault)
#   130      fault code

param(
    [int]$Port = 502,
    [int]$SlaveCount = 8,
    [double]$CurrentMa = 10.0,
    [double]$VoltageV = 3.0,
    [double]$FurnaceC = 25.3
)

$ErrorActionPreference = "Stop"

$currentRaw = [int]($CurrentMa * 10)
$voltageRaw = [int]($VoltageV * 10)
$furnaceRaw = [int]($FurnaceC * 10)

# One background job per slave. Each job owns one TCP listener and one register image.
$slaveScript = {
    param($ip, $port, $currentRaw, $voltageRaw, $furnaceRaw, $devIndex)

    # 131 holding registers, filled the way a real power supply would look.
    function New-Registers {
        $regs = New-Object 'int[]' 131
        $regs[0] = $furnaceRaw
        for ($k = 1; $k -le 64; $k++) { $regs[$k] = $currentRaw }
        for ($k = 65; $k -le 128; $k++) { $regs[$k] = $voltageRaw }
        $regs[129] = 1
        $regs[130] = 0
        return ,$regs
    }

    # TCP gives you whatever arrived, not the count you asked for. Loop until it is all here.
    function Read-Exactly($stream, [int]$count) {
        $buffer = New-Object 'byte[]' $count
        $got = 0
        while ($got -lt $count) {
            $n = $stream.Read($buffer, $got, $count - $got)
            if ($n -le 0) { return $null }
            $got += $n
        }
        return ,$buffer
    }

    function Test-Range([int]$start, [int]$qty) {
        if ($qty -le 0) { return $false }
        if ($start -lt 0) { return $false }
        if (($start + $qty) -gt 131) { return $false }
        return $true
    }

    # Start time of this job, used to make readings drift slowly so the UI looks alive.
    $t0 = Get-Date

    # Value by register address (raw, divide by 10 in the app).
    #   0        furnace 250..258  (25.0 .. 25.8 C, slowly drifting)
    #   1..64    current 97..103   (9.7 .. 10.3 mA); special: device 3 channel 5 = 0 (open),
    #                               device 5 channel 10 = 150 (15.0 mA, over current)
    #   65..128  voltage 28..32    (2.8 .. 3.2 V)
    #   129      power state 1 = outputting
    #   130      fault code 0
    function Get-Value([int]$addr) {
        $sec = [int]((Get-Date) - $t0).TotalSeconds

        if ($addr -eq 0) { return 250 + ($sec % 9) }
        if ($addr -ge 1 -and $addr -le 64) {
            if ($devIndex -eq 3 -and $addr -eq 5) { return 0 }
            if ($devIndex -eq 5 -and $addr -eq 10) { return 150 }
            $drift = (($sec * 3 + $addr * 7) % 7) - 3
            return 100 + $drift
        }
        if ($addr -ge 65 -and $addr -le 128) {
            if ($devIndex -eq 6 -and $addr -eq 72) { return 33 }      # 6 号第 8 路 = 3.3 V，模拟电压超限
            $drift = [int]((($sec / 2) + $addr) % 3)                  # 2.9 ~ 3.1 V，不触发电压报警
            return 29 + $drift
        }
        if ($addr -eq 129) { return 1 }
        if ($addr -eq 130) { return 0 }
        return 0
    }

    $endpoint = New-Object System.Net.IPEndPoint([System.Net.IPAddress]::Parse($ip), $port)
    $listener = New-Object System.Net.Sockets.TcpListener($endpoint)
    $listener.Start()
    Write-Host ("[" + $ip + ":" + $port + "] listening")

    while ($true) {
        $client = $listener.AcceptTcpClient()
        $client.NoDelay = $true
        $stream = $client.GetStream()
        $regs = New-Registers

        while ($true) {
            # MBAP header: transaction(2) protocol(2) length(2) unit(1)
            $head = Read-Exactly $stream 7
            if ($head -eq $null) { break }

            $length = ($head[4] -shl 8) -bor $head[5]
            $pduLength = $length - 1
            if ($pduLength -le 0) { break }

            $pdu = Read-Exactly $stream $pduLength
            if ($pdu -eq $null) { break }

            $fc = $pdu[0]
            $reply = $null

            if ($fc -eq 3) {
                $start = ($pdu[1] -shl 8) -bor $pdu[2]
                $qty = ($pdu[3] -shl 8) -bor $pdu[4]
                $ok = Test-Range $start $qty
                if ($ok -eq $false) {
                    $reply = New-Object 'byte[]' 2
                    $reply[0] = 0x83
                    $reply[1] = 2
                }
                else {
                    $reply = New-Object 'byte[]' (2 + 2 * $qty)
                    $reply[0] = 0x03
                    $reply[1] = [byte](2 * $qty)
                    $p = 2
                    for ($k = 0; $k -lt $qty; $k++) {
                        $v = Get-Value ($start + $k)
                        $reply[$p] = [byte](($v -shr 8) -band 0xFF)
                        $reply[$p + 1] = [byte]($v -band 0xFF)
                        $p += 2
                    }
                }
            }
            elseif ($fc -eq 16) {
                $start = ($pdu[1] -shl 8) -bor $pdu[2]
                $qty = ($pdu[3] -shl 8) -bor $pdu[4]
                $ok = Test-Range $start $qty
                if ($ok -eq $false) {
                    $reply = New-Object 'byte[]' 2
                    $reply[0] = 0x90
                    $reply[1] = 2
                }
                else {
                    $p = 6
                    for ($k = 0; $k -lt $qty; $k++) {
                        $regs[$start + $k] = (($pdu[$p] -shl 8) -bor $pdu[$p + 1])
                        $p += 2
                    }
                    $reply = New-Object 'byte[]' 5
                    $reply[0] = 0x10
                    $reply[1] = $pdu[1]
                    $reply[2] = $pdu[2]
                    $reply[3] = $pdu[3]
                    $reply[4] = $pdu[4]
                }
            }
            elseif ($fc -eq 6) {
                $addr = ($pdu[1] -shl 8) -bor $pdu[2]
                $ok = Test-Range $addr 1
                if ($ok -eq $false) {
                    $reply = New-Object 'byte[]' 2
                    $reply[0] = 0x86
                    $reply[1] = 2
                }
                else {
                    $regs[$addr] = ($pdu[3] -shl 8) -bor $pdu[4]
                    $reply = New-Object 'byte[]' 5
                    $reply[0] = 0x06
                    $reply[1] = $pdu[1]
                    $reply[2] = $pdu[2]
                    $reply[3] = $pdu[3]
                    $reply[4] = $pdu[4]
                }
            }

            if ($reply -eq $null) { break }

            # MBAP reply: same transaction id, protocol 0, length, unit id
            $frame = New-Object 'byte[]' (7 + $reply.Length)
            $frame[0] = $head[0]
            $frame[1] = $head[1]
            $frame[2] = 0
            $frame[3] = 0
            $frameLength = $reply.Length + 1
            $frame[4] = [byte](($frameLength -shr 8) -band 0xFF)
            $frame[5] = [byte]($frameLength -band 0xFF)
            $frame[6] = $head[6]
            [System.Array]::Copy($reply, 0, $frame, 7, $reply.Length)

            $stream.Write($frame, 0, $frame.Length)
            $stream.Flush()
        }

        $client.Close()
        Write-Host ("[" + $ip + ":" + $port + "] client closed")
    }
}

for ($i = 1; $i -le $SlaveCount; $i++) {
    $ip = "127.0.0." + $i
    Start-Job -ScriptBlock $slaveScript -ArgumentList $ip, $Port, $currentRaw, $voltageRaw, $furnaceRaw, $i | Out-Null
}

Write-Host ""
Write-Host ("Started " + $SlaveCount + " fake slaves: 127.0.0.1 .. 127.0.0." + $SlaveCount + " port " + $Port)
Write-Host ("Current = " + $CurrentMa + " mA, Voltage = " + $VoltageV + " V, Furnace = " + $FurnaceC + " C")
Write-Host ""
Write-Host "Keep this window open. Press Ctrl+C to stop."

while ($true) {
    Start-Sleep -Seconds 30
    $running = (Get-Job -State Running).Count
    Write-Host ("still running: " + $running + " slaves")
}