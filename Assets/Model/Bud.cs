using System;
using Model.Class.Automaton;
using UnityEngine;
using Random = System.Random;

namespace Model
{
   //TODO: 常用函数 静态方法
   // 面向使用者的定义一个芽
   public class Bud
   {
      private readonly Func<int,float> _randomRatio;// Non-periodic random aspect - $B(i)$ - $b$ 随机比 b
      private readonly Func<int,float> _mortalityRatio; // Mortality - $C(i)$ - c 生存率 c
      private readonly bool[] _rhythmRatio; // rhythm ratio - $w_\varphi$ 节律比 w
      private readonly Func<int, Phytomer, float> _branchingIntensity;// Branching intensity - F(i) - a 分支强度 a
      private readonly Func<Vector3,float> _lightRatio; // Light ratio - $L()$ 光线随机比 l
      private readonly Automaton _automaton; // 自动机

      public Bud(
         Func<int,float> randomRatio,
         Func<int,float> mortalityRatio,
         bool[] rhythmRatio,
         Func<int, Phytomer, float> branchingIntensity,
         Func<Vector3,float> lightRatio,
         Automaton automaton
         )
      {
         _randomRatio = randomRatio;
         _mortalityRatio = mortalityRatio;
         _rhythmRatio = rhythmRatio;
         _branchingIntensity = branchingIntensity;
         _lightRatio = lightRatio;
         _automaton = automaton;
      }
   }
   // 程序内部为了自身处理所用的Bug对象
   internal class BuiltinBug
   {
      private Bud _bud;
      
      private int _age = 0;
      private readonly System.Random _random;
      private bool _isViability = true;
      private readonly Phytomer _parent = null;

      // 实例化
      public BuiltinBug(Bud bud, int randomSeed,Phytomer parent)
      {
         _bud = bud;
         _random = new Random(randomSeed);
         _parent = parent;
      }
      
      // 
   }
}