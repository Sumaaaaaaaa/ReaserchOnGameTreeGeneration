namespace Model.Class.Automaton
{
    public class OutAutomaton : Automaton
    {
    
        private readonly InAutomaton[] _vertices; // 顶点列表
    
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="vertices">顶点内自动机对象</param>
        /// <param name="repeatTimes">重复次数数组</param>
        /// <param name="adjMat">邻接矩阵</param>
        /// <param name="entranceIndex">指定入口态编号</param>
        /// <param name="randomSeed">随机种</param>
        /// <param name="dataCheck">是否对输入的数据进行检查，建议在确认输入的数据不会有错误的时候，传入false节省检查的开销</param>
        public OutAutomaton(InAutomaton[] vertices, int[] repeatTimes, float[,] adjMat, int entranceIndex,int randomSeed,
            bool dataCheck = true) :
            base(repeatTimes, adjMat, entranceIndex, randomSeed)
        {
            if (dataCheck)
            {
                DataCheck<InAutomaton>(vertices,repeatTimes,adjMat,entranceIndex);
            }
            _vertices = vertices;
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
                _stateNow = _enterStateIndex;
                for (var i = 0; i < times; i++)
                {
                    results[i] = _vertices[_stateNow].Expansion();
                }
                return results;
            }
        
            //重复
            if (_stateRepeatTime < _repeatTimes[_stateNow])
            {
                _stateRepeatTime ++;
                for (var i = 0; i < times; i++)
                {
                    results[i] = _vertices[_stateNow].Expansion();
                }
                return results;
            }
        
            // 跳转状态
            var sumValue = 0.0f;
            for (var i = 0; i < _vertices.Length; i++)
            {
                sumValue += _adjMat[_stateNow, i];
                if (_random.NextDouble() <= sumValue)
                {
                    _stateNow = i;
                    _stateRepeatTime = 0;
                    for (var t = 0; t < times; t++)
                    {
                        results[t] = _vertices[_stateNow].Expansion();
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


