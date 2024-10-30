namespace Model.Class.Automaton
{
    public class InAutomaton : Automaton
    {
        public readonly Phytomer[] Vertices; // 顶点列表
        
        public InAutomaton(Phytomer[] vertices,int[] repeatTimes, float[,] adjMat,int entranceIndex, bool dataCheck = true)
            :base(repeatTimes,adjMat,entranceIndex)
        {
            if (dataCheck)
            {
                DataCheck(vertices,repeatTimes,adjMat,entranceIndex);
            }
            
            Vertices = vertices;
        }
    }
}