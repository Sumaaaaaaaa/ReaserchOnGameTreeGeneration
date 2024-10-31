#nullable enable
namespace Model.Automaton
{
    internal class BuiltinInAutomaton : BuiltinAutomaton
    {
        private readonly InAutomaton _inAutomaton;

        public BuiltinInAutomaton(InAutomaton inAutomaton, int randomSeed) : base(randomSeed)
        {
            _inAutomaton = inAutomaton;
        }
        public Phytomer? Expansion()
        {
            // 芽死亡时，怎么都不会产生新的对象了。
            if (_budDead) return null;
        
            // 入口
            if (_stateNow == -1)
            {
                _stateNow = _inAutomaton.EnterStateIndex;
                return _inAutomaton.Vertices[_stateNow];
            }
        
            // 重复
            if (_stateRepeatTime < _inAutomaton.RepeatTimes[_stateNow])
            {
                _stateRepeatTime++;
                return _inAutomaton.Vertices[_stateNow];
            }

            // 跳转状态
            var sumValue = 0.0f;
            for (var i = 0; i < _inAutomaton.Vertices.Length; i++)
            {
                sumValue += _inAutomaton.AdjMat[_stateNow, i];
                if (_random.NextDouble() <= sumValue)
                {
                    _stateNow = i;
                    _stateRepeatTime = 0;
                    return _inAutomaton.Vertices[_stateNow];
                }
            }
        
            // 没有能成功扩展，返回空，等于芽死亡
            _budDead = true;
            return null;
        }
    }
}