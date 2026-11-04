using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipAgingCheck.Domain.Common
{
    public static class CurveCalculator
    {
        /// <summary>
        /// 获取当前温度
        /// </summary>
        /// <param name="currentCurveSteps">曲线</param>
        /// <param name="currentTemp">当前温度</param>
        /// <param name="elapsedTime">已经过去的时间</param>
        /// <returns></returns>
        public static double GetTargetTemperature(List<TempCurveStep> currentCurveSteps,double currentTemp,double elapsedTime)
        {
            if (currentCurveSteps == null || currentCurveSteps.Count == 0  )
            {
                return currentTemp;
            }
            //已经过去的时间
            double elapsed = elapsedTime;
            if (elapsed < 0) 
            { 
            
                elapsed = 0;
            }
            double temp = currentTemp;
            for (int i = 0; i < currentCurveSteps.Count; i++)
            {
                TempCurveStep curveStep = currentCurveSteps[i];
                //总时长
                double totalTime = curveStep.RampMinutes+curveStep.HoldMinutes;
                bool nowStep = elapsed<= totalTime;//如果当前时间还在这一段
                if (nowStep)
                {
                    bool inHolding = elapsed >= curveStep.RampMinutes;//如果现在恒温阶段直接返回温度
                    if (inHolding)
                    {
                        return curveStep.TargetTemp;

                    }
                    //如果在升温或者降温阶段
                    double ratio =elapsed/curveStep.RampMinutes;
                    double gap = 0.0;
                    if (curveStep.TargetTemp==temp)
                    {
                         gap = temp-curveStep.TargetTemp;
                    }
                    else
                    {
                        gap = curveStep.TargetTemp - temp;
                    }
                  
                    return temp+gap*ratio;

                }
                elapsed = elapsed - totalTime;
                temp=curveStep.TargetTemp;


            }
            return temp;
        }
      




    }
}
