#nullable enable
using JetBrains.Annotations;
using Random = System.Random;

namespace UnlimitedGreen
{
    internal abstract class EntityAutomaton<T>
    {
        protected Random Random;
        protected int StateNow  = -1; // 当前处于的位置
        protected int StateRepeatTime = 0 ; // 重复次数
        protected bool BudDead = false; // 芽死亡，转True后，无论多少次触发，结果永远为 Null
        protected int EntranceIndex; // 入口序号

        protected T Automaton;
        
        protected EntityAutomaton(T automaton,Random random, int entranceIndex)
        {
            Automaton = automaton;
            Random = random;
            EntranceIndex = entranceIndex;
        }
    }

    internal class EntityInAutomaton : EntityAutomaton<InAutomaton>
    {
        
        public EntityInAutomaton([NotNull] InAutomaton automaton, [NotNull] Random random, int entranceIndex) 
            : base(automaton, random, entranceIndex)
        {
        }

        public Phytomer? Expansion()
        {
            //如果芽已经死亡（当自动机可能性的总值不为1时，可能发生），则返回空的值
            if (BudDead) return null;
            
            // 入口
            if (StateNow == -1)
            {
                StateNow = EntranceIndex;
                return Automaton.Vertices[StateNow];
            }
            
            // 重复
            if (StateRepeatTime < Automaton.RepeatTimes[StateNow])
            {
                StateRepeatTime++;
                return Automaton.Vertices[StateNow];
            }
            
            // 状态跳转
            var sumValue = 0.0f;
            var randomValue = Random.NextDouble();
            for (var i = 0; i < Automaton.Vertices.Length; i++)
            {
                sumValue += Automaton.AdjMat[StateNow, i];
                if (randomValue > sumValue) continue;
                StateNow = i;
                StateRepeatTime = 0;
                return Automaton.Vertices[StateNow];
            }
            BudDead = true;
            return null;
        }

    }

    internal class EntityDualScaleAutomaton : EntityAutomaton<DualScaleAutomaton>
    {
        private readonly EntityInAutomaton[] _builtinInAutomatas;
        
        public EntityDualScaleAutomaton([NotNull] DualScaleAutomaton automaton, [NotNull] Random random, int entranceIndex) 
            : base(automaton, random, entranceIndex)
        {
            
            var length = Automaton.Vertices.Length;
            _builtinInAutomatas = new EntityInAutomaton[length];

            for (var i = 0; i < length; i++)
            {
                _builtinInAutomatas[i] = new EntityInAutomaton(Automaton.Vertices[i], random, 0);
            }
        }
        
        /// <summary>
        /// 进行一次自动机处理，即进行一次芽扩展。
        /// </summary>
        /// <returns>返回叶元对象数组，可能为null，意味着自动机导致的芽死亡。</returns>
        public (Phytomer phytomer,int indexNow)? Expansion()
        {
            //如果芽已经死亡（当自动机可能性的总值不为1时，可能发生），则返回空的值
            if (BudDead) return null;
            
            // 入口
            if (StateNow == -1)
            {
                StateNow = EntranceIndex;
                var result = _builtinInAutomatas[StateNow].Expansion();
                return result is null ? null : (result, StateNow);
            }
            
            // 重复
            if (StateRepeatTime < Automaton.RepeatTimes[StateNow])
            {
                StateRepeatTime++;
                var result = _builtinInAutomatas[StateNow].Expansion();
                return result is null ? null : (result, StateNow);
            }
            
            // 状态跳转
            var sumValue = 0.0f;
            var randomVale = Random.NextDouble();
            for (var i = 0; i < Automaton.Vertices.Length; i++)
            {
                sumValue += Automaton.AdjMat[StateNow, i];
                if (randomVale > sumValue) continue;
                StateNow = i;
                StateRepeatTime = 0;
                var result = _builtinInAutomatas[StateNow].Expansion();
                return result is null ? null : (result, StateNow);
            }
            
            // 芽死亡
            BudDead = true;
            return null;
        }
    }
}