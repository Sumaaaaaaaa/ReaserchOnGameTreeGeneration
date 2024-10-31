using Random = System.Random;

namespace Model.Automaton
{
    // 内部处理的实际表达芽的类
    internal abstract class BuiltinAutomaton
    {
        
        protected Random _random;
        protected int _stateNow  = -1; // 当前处于的位置
        protected int _stateRepeatTime = 0 ; // 重复次数
        protected bool _budDead = false; // 芽死亡，转True后，无论多少次触发，结果永远为 Null

        protected BuiltinAutomaton(int randomSeed)
        {
            _random = new Random(randomSeed);
        }
    
    }
}
