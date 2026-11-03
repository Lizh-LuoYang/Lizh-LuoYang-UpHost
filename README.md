# ChipAgingCheck

光芯片老化/测试设备的上位机，C# WinForms + .NET Framework 4.7.2，走 Modbus TCP 采集。

一台设备带 8 个可编程电源，每个电源 64 路，总共 512 路通道同时监控。界面是自绘的格子，一眼能看出哪一路在报警。

业余项目，拿来练工业上位机的写法。没上过产线，别直接拿去用。

第一次打开要还原 NuGet 包（右键解决方案 →「还原 NuGet 包」），不然会报一堆 CS0246。

## 现在能做什么

- 同时连 8 台设备，一次读 125 个寄存器，断线后按 300ms×n 退避重连，最多等 1.5 秒
- 512 格看板，绿=在线、红=报警、灰=离线，1 秒刷一次
- 报警判 5 类：炉温、电流超限、电流开路、电压超限、电源故障和错误码。同一路同一类型只报一条，恢复了自己撤掉
- 配方可以改温度上下限、保持时间、容差、电流阈值系数，内置降温/恒温/回温三套
- 配置存 JSON，文件没了、写坏了、版本号对不上，都退回默认值，不会崩
- 每 60 秒把 512 路快照写进 CSV，带 BOM，Excel 打开中文不乱码；报警单独占一行
- 批次号按时间生成，断电重启能认出没跑完的那一批，接着往里写

## 目录

```
ChipAgingCheck.UI       界面：主窗口 + 配方配置 + 设备配置
ChipAgingCheck.Domain   采集、报警、配置、落盘，不引用 UI
ChipAgingCheck.Manager  目前是空的，留着放多设备调度
tools/                  假从站，没硬件也能跑
```

Domain 不认识 UI，界面靠事件订阅采集层，跨线程的地方用 BeginInvoke 回主线程。

## 跑起来

不用真设备：

```powershell
powershell -ExecutionPolicy Bypass -File tools\fake-slaves.ps1
```

然后 VS2022 打开 `ChipAgingCheck.sln`，F5。

假从站里埋了三个故障，按启动后应该看到：

- 第 133 号位开路（0 mA）
- 第 266 号位电流超限（15 mA）
- 第 328 号位电压超限（3.3 V）

对应三格变红，报警框三条，日志框 8 条「连接成功」。

## 文件放哪

```
%ProgramData%\ChipAgingCheck\
├─ Config\config.json      设备参数和配方
├─ Data\{批次号}.csv       每 60 秒一轮，一轮 512 行
├─ batch.json              当前批次，断电恢复靠它
└─ Logs\{日期}\log_*.log   10 MB 滚一次
```

ProgramData 写不进去就退到程序目录，还不行退到 %TEMP%，三级兜底。

## 报警怎么判

- 炉温：超出 `TempHigh/TempLow ± TempTolerance`，整炉只报一条
- 电流超限：超过 `标称电流 × CurrentOverMultiplier`，默认 1.2
- 电流开路：低于 `标称电流 × CurrentNearZeroPercent`，默认 10%
- 电压超限：超过 `合规电压 × VoltageOverMultiplier`，默认 1.05
- 电源故障：状态寄存器 = Malfunction，整台 64 格全红
- 错误码：错误码寄存器不等于 0

去重靠 `HasAlarm(type, index)`，恢复靠 `RemoveChannelAlarm`。界面读报警必须走 `GetAlarmSnapshot()`，它里面加了锁，直接遍历 `Alarms` 会在采集线程往里加的时候炸掉。

## 还没做的

- 温度曲线的三段推进没写。`Recipe.Curve` 的数据结构搭好了，推进逻辑和曲线面板空着——假从站的炉温是常数，没法验证
- 没有用户权限、历史查询、报表导出，也没接 MES/SECS
- 窗口尺寸是写死的，按 125% 缩放的 1920×1080 调过，换分辨率要重排
- 改完设备参数得重新按一次「启动」才生效

## 环境

Windows + .NET Framework 4.7.2 + VS2022。依赖 NModbus 和 Newtonsoft.Json，都是 PackageReference，编译时自动还原。

MIT。
## 后续计划
- 温度曲线面板