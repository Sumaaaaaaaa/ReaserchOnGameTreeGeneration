using System;
using Model.Automaton;
using UnityEngine;
using Random = System.Random;

namespace Model.Bud
{
    internal class Builtin_Bud
    {
             private Model.Bud.Bud _bud;
      
      private int _age = 0;
      private readonly System.Random _random;
      private bool _isViability = true;
      private readonly Phytomer _parent = null;

      private readonly bool _isUsingDualAutomaton;
      private readonly BuiltinInAutomaton _inAutomaton = null;
      private readonly BuiltinOutAutomaton _outAutomaton = null;
      
      private Vector3 _position = Vector3.zero;
      private Vector3 Position
      {
         get
         {
            Debug.LogWarning("注意！！芽的任何关于位置的功能还没有实现");
            return _position;
         }
      }
      //TODO: 关于芽的位置的功能。

      // 实例化
      public Builtin_Bud(Model.Bud.Bud bud, Phytomer parent, int randomSeed)
      {
         _bud = bud;
         _random = new Random(randomSeed);
         _parent = parent;
         
         // 实例化自动机
         switch (_bud.Automaton)
         {
            case OutAutomaton outAutomaton:
               _isUsingDualAutomaton = true;
               _outAutomaton = new BuiltinOutAutomaton(outAutomaton, randomSeed + 1);
               break;
            case InAutomaton inAutomaton:
               _isUsingDualAutomaton = false;
               _inAutomaton = new BuiltinInAutomaton(inAutomaton, randomSeed + 1);
               break;
         }
      }
      
      // 扩展一次
      // 直接返回的表示芽在这次处理后是否依旧存活，out 返回的是结果，
      // 需要注意的是，就算发生了芽死亡导致bool为false，所返回的非null得phytomer依旧是可信的。
      public bool Expansion(out Phytomer[] results)
      {
         
         // 增加一级年龄
         _age++;
         
         // 以下几种情况都是跳过发育，所以结果一定为空
         results = new Phytomer[]{};
         
         // 如果芽死亡了，无论如何扩展都是空的
         if (!_isViability)
         {
            _isViability = false;
            return false;
         }
         
         // 节奏比判定
         if (_bud.RhythmRatio[(_age - 1) % _bud.RhythmRatio.Length] == false)
         {
            // 当前为休眠节律，所以跳过后续判定
            return true;
         }
         
         // 启动判定
         var ratio = _bud.BranchingIntensity(_age, _parent);
         // 当分支强度计算结果为负数时，将会直接判定为芽死亡
         if (ratio < 0) return false;
         if (_random.NextDouble() > ratio)
         {
            // 判定失败则跳过
            return true;
         }
         
         // 存活率判定
         if (_random.NextDouble() > _bud.MortalityRatio(_age))
         {
            // 判定芽死亡
            _isViability = false;
            return false;
         }
         
         // 随机处理
         if (_random.NextDouble() > _bud.RandomRatio(_age))
         {
            // 判定失败则跳过
            return true;
         }
         
         // 光判定
         if (_random.NextDouble() > _bud.LightRatio(Position))
         {
            return true;
         }
         
         // 通过了以上检测后，就会进入自动机部分
         if (_isUsingDualAutomaton)
         {
            // 使用的是双尺度自动机
            var runTimes = _bud.ExpansionTimes();
            var result = _outAutomaton.Expansion(runTimes);
            for (var i = 0; i < runTimes; i++)
            {
               if (result[i] is not null) continue;
               results = new Phytomer?[i+1];
               // 为null，说明芽死了。
               Array.Copy(result, results, i + 1);
               _isViability = false;
               return false;
            }
            results = result;
            return true;
         }
         else
         {
            // 使用的单尺度自动机
            var result = _inAutomaton.Expansion();
            if (result is null)
            {
               //为空，说明芽死了
               _isViability = false;
               return false;
            }
            results = new[] { result };
            return true;
         }
      }

    }
}