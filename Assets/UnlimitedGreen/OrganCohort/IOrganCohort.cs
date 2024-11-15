namespace UnlimitedGreen
{
    internal interface IOrganCohort
    {
        // 计算汇总和
        float CalculateSinkSum(int plantAge);
        // 分配
        void Allocate(int plantAge,float producedBiomass,float sinkSum);
        // 增加
        // void Add<T>(int plantAge,T organ);
        //年龄增长（导致存储内容的变更）
        void IncreaseAge(int plantAge);


    }
}