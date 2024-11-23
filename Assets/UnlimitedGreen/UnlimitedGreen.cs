using UnityEngine;

namespace UnlimitedGreen
{
    public class UnlimitedGreen
    {
        public static void Test()
        {
            /*var leafData = new LeafData(
                maxPhysiologicalAge: 3,
                leafAllometryE: 1, //厚度
                extinctionCoefficient: 1, //消光系数
                projectionArea: 1, // 投影面积
                waterUseEfficiency: 1, // 水资源数
                sourceValidCycles: 3,
                sinkValidCycles: 2,
                (phy,age) => phy+age*0.1f // 汇函数
            );
            var leafC = new LeafCohort(leafData);
            Debug.Log(leafC);
            
            var age = 0;
            var sinkSum = 0f;
            var biomass = 10f;
            
            // --1--
            Debug.Log(age++);
            leafC.Add(3,new EntityLeaf());
            leafC.Add(2,new EntityLeaf());
            leafC.Add(1,new EntityLeaf());
            Debug.Log(leafC);
            sinkSum = leafC.CalculateSinkSum(age);
            Debug.Log($"SinkSum = {sinkSum}");
            Debug.Log(leafC);
            leafC.Allocate(age,biomass,sinkSum);
            Debug.Log(leafC);
            biomass = leafC.Production(1);
            Debug.Log(biomass);
            leafC.IncreaseAge(age);
            Debug.Log(leafC);
            
            Debug.Log(age++);
            leafC.Add(2,new EntityLeaf());
            leafC.Add(2,new EntityLeaf());
            Debug.Log(leafC);
            sinkSum = leafC.CalculateSinkSum(age);
            Debug.Log($"SinkSum = {sinkSum}");
            Debug.Log(leafC);
            leafC.Allocate(age,biomass,sinkSum);
            Debug.Log(leafC);
            biomass = leafC.Production(1);
            Debug.Log(biomass);
            leafC.IncreaseAge(age);
            Debug.Log(leafC);
            
            Debug.Log(age++);
            Debug.Log(leafC);
            sinkSum = leafC.CalculateSinkSum(age);
            Debug.Log($"SinkSum = {sinkSum}");
            Debug.Log(leafC);
            leafC.Allocate(age,biomass,sinkSum);
            Debug.Log(leafC);
            biomass = leafC.Production(1);
            Debug.Log(biomass);
            leafC.IncreaseAge(age);
            Debug.Log(leafC);
            
            Debug.Log(age++);
            leafC.Add(2,new EntityLeaf());
            leafC.Add(2,new EntityLeaf());
            Debug.Log(leafC);
            sinkSum = leafC.CalculateSinkSum(age);
            Debug.Log($"SinkSum = {sinkSum}");
            Debug.Log(leafC);
            leafC.Allocate(age,biomass,sinkSum);
            Debug.Log(leafC);
            biomass = leafC.Production(1);
            Debug.Log(biomass);
            leafC.IncreaseAge(age);
            Debug.Log(leafC);*/

        }
    }
}