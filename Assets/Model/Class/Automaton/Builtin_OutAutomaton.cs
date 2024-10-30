namespace Model.Class.Automaton
{
    
    internal class BuiltinOutAutomaton : BuiltinAutomaton
    {

        private readonly OutAutomaton _outAutomaton;
        private readonly BuiltinInAutomaton[] _builtinInAutomatas;
        
        public BuiltinOutAutomaton(OutAutomaton outAutomaton,int randomSeed) : base(randomSeed)
        {
            _outAutomaton = outAutomaton;
            
            // 将抽象的顶点矩阵转换一个内部的实体处理的矩阵对象
            var length = _outAutomaton.Vertices.Length;

            _builtinInAutomatas = new BuiltinInAutomaton[length];

            for (int i = 0; i < length; i++)
            {
                _builtinInAutomatas[i] = new BuiltinInAutomaton(_outAutomaton.Vertices[i], randomSeed + 1 + i);
            }

        }
    
        /// <summary>
        /// 进行一次自动机处理，即进行一次芽扩展。
        /// </summary>
        /// <returns>返回叶元对象数组，可能为null，意味着自动机导致的芽死亡。</returns>
        public Phytomer?[] Expansion(int times)
        {
            // List<Phytomer?> results = new List<Phytomer?>();
            Phytomer?[] results = new Phytomer?[times];
        
            // 芽死亡时，怎么都不会产生新的对象了。
            if (_budDead) return null;
        
            // 入口
            if (_stateNow == -1)
            {
                _stateNow = _outAutomaton.EnterStateIndex;
                for (var i = 0; i < times; i++)
                {
                    results[i] = _builtinInAutomatas[_stateNow].Expansion();
                }
                return results;
            }
        
            //重复
            if (_stateRepeatTime < _outAutomaton.RepeatTimes[_stateNow])
            {
                _stateRepeatTime ++;
                for (var i = 0; i < times; i++)
                {
                    results[i] = _builtinInAutomatas[_stateNow].Expansion();
                }
                return results;
            }
        
            // 跳转状态
            var sumValue = 0.0f;
            for (var i = 0; i < _outAutomaton.Vertices.Length; i++)
            {
                sumValue += _outAutomaton.AdjMat[_stateNow, i];
                if (_random.NextDouble() <= sumValue)
                {
                    _stateNow = i;
                    _stateRepeatTime = 0;
                    for (var t = 0; t < times; t++)
                    {
                        results[t] = _builtinInAutomatas[_stateNow].Expansion();
                    }
                    return results;
                }
            }
        
            // 没有能成功扩展，返回空，等于芽死亡
            _budDead = true;
            return null;
        }
    }
}


