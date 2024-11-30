using UnityEngine;
using Random = System.Random;

namespace UnlimitedGreen
{
    internal class EntityBud
    {
        private Bud _bud;

        private int _physiologicalAge;
        private int _parentPhysiologicalAge;
        private int _birthCycle;
        
        private Random _random;
        
        private bool _isViability = true; 
        //OPT: 根据植物的逻辑，收到false 的一瞬间就会把种子删了，所以这里也许可以省去是否存活的判断
        private bool _isBranched = false; // 用于记录时候已经分支强度判定过了，判定成功过了之后的处理就不需要判定了。

        private readonly EntityDualScaleAutomaton _entityDualScaleAutomaton;

        internal EntityBud(Bud bud, int physiologicalAge,int parentPhysiologicalAge, int birthCycle,
            Random random, DualScaleAutomaton dualScaleAutomaton)
        {
            _bud = bud;
            _physiologicalAge = physiologicalAge;
            _parentPhysiologicalAge = parentPhysiologicalAge;
            _birthCycle = birthCycle;
            _random = random;
            _entityDualScaleAutomaton = new EntityDualScaleAutomaton
                (dualScaleAutomaton, random, _physiologicalAge - 1);
        }

        internal bool Expansion(Vector3 worldPosition,int planetAge,out (Phytomer phytomer,int indexNow)? phytomer)
        {
            
            // 首先将 phytomer 的返回设为 null
            phytomer = null;
            
            // 芽已经死亡，无论如何都不会在产生新的叶元了
            if (!_isViability)
            {
                return false;
            }
            
            var age = GenericFunctions.CalculateAge(planetAge, _birthCycle);
            
            // 节律判定
            if (_bud.RhythmRatio[(age - 1) % _bud.RhythmRatio.Length] == false)
            {
                return true;
            }
            
            // 启动判定
            if (!_isBranched)
            {
                var ratio = _bud.BranchingIntensity(age,_parentPhysiologicalAge); //输入：父体生理年龄，本体生理年龄，个体年龄
                if (ratio < 0)
                {
                    _isViability = false;
                    return false;
                }
                if (_random.NextDouble() > ratio)
                {
                    return true;
                }
                _isBranched = true;
            }
            
            // 存活判定
            if (_random.NextDouble() > _bud.ViabilityRatio(age))
            {
                _isViability = false;
                return false;
            }
            
            // 随机处理
            if (_random.NextDouble() > _bud.RandomRatio(age))
            {
                return true;
            }
            
            // 光判断
            if (_random.NextDouble() > _bud.LightRatio(worldPosition))
            {
                return true;
            }
            
            // 通过所有的检测，说明可以产出新的叶元
            var result = _entityDualScaleAutomaton.Expansion();
            if (result is null)
            {
                _isViability = false;
                return false;
            }

            phytomer = result;
            return true;
        }
    }
}