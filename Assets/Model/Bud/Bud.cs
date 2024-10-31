using System;
using Model.Automaton;
using UnityEngine;
using Random = System.Random;

namespace Model.Bud
{
   //TODO: 常用函数 静态方法
   // 面向使用者的定义一个芽
   public class Bud
   {
      public readonly Func<int,float> RandomRatio;// Non-periodic random aspect - $B(i)$ - $b$ 随机比 b
      public readonly Func<int,float> MortalityRatio; // Mortality - $C(i)$ - c 生存率 c
      public readonly bool[] RhythmRatio; // rhythm ratio - $w_\varphi$ 节律比 w
      public readonly Func<int, Phytomer, float> BranchingIntensity;// Branching intensity - F(i) - a 分支强度 a
      public readonly Func<Vector3,float> LightRatio; // Light ratio - $L()$ 光线随机比 l
      public readonly Automaton.Automaton Automaton; // 自动机 //TODO: 这样的写法能兼容同时两种自动机吗
      public readonly Func<int> ExpansionTimes; // 当模拟尺度为GU时，需要通过这个计算生成的Phytomer数量
      /// <summary>
      /// 描述一个芽
      /// </summary>
      /// <param name="randomRatio">随机比</param>
      /// <param name="mortalityRatio">生存率计算方程</param>
      /// <param name="rhythmRatio">通过true和false的数组来描述节律比</param>
      /// <param name="branchingIntensity">分支强度，函数可以支持两个输入，年龄和分支源对象，注意，当结果为负数时，将会直接判定为芽死亡</param>
      /// <param name="lightRatio">光比函数，支持一个输入的位置</param>
      /// <param name="automaton">自动机对象</param>
      /// <param name="expansionTimes">当自动机为双尺度时，需要通过这个函数计算生成的Phytomer数量</param>
      /// <param name="dataCheck">是否检查数据</param>
      public Bud(
         Func<int,float> randomRatio,
         Func<int,float> mortalityRatio,
         bool[] rhythmRatio,
         Func<int, Phytomer, float> branchingIntensity,
         Func<Vector3,float> lightRatio,
         Automaton.Automaton automaton,
         Func<int> expansionTimes = null,
         bool dataCheck = true
         )
      {
         RandomRatio = randomRatio;
         MortalityRatio = mortalityRatio;
         RhythmRatio = rhythmRatio;
         BranchingIntensity = branchingIntensity;
         LightRatio = lightRatio;
         Automaton = automaton;
         ExpansionTimes = expansionTimes;
         if (!dataCheck) return;
         DataCheck();
      }
      private void DataCheck()
      {
         // 几个方法不能为null
         if (RandomRatio is null | MortalityRatio is null | BranchingIntensity is null | LightRatio is null)
         {
            throw new Exception("除了<expansionTimes以外的方法一律不能为空。");
         }
         
         // 当使用双持度自动机的时候，必须保证有一个定义进行几次自动机处理的方法
         if (Automaton is OutAutomaton && ExpansionTimes is null)
         {
            throw new Exception("当自动机为双尺度自动机时，必须定义ExpansionTimes");
         }

         if (Automaton is InAutomaton & ExpansionTimes is not null)
         {
            throw new Exception("当使用单尺度时，不该对ExpansionTimes进行定义");
         }
         
         // rhythmRatio 不能没有任何的描述
         if (RhythmRatio.Length == 0)
         {
            throw new Exception("rhythmRatio不能为空，必须具有至少一个值");
         }
         return;
      }
   }
   // 程序内部为了自身处理所用的Bug对象
   // 注意：此处的Age不是按照程序逻辑中常见的以0为第一周期的模式，而是按照数学描述中，从1开始算第一次周期的描述模式
   
}