using System;
using UnityEngine;
using UnlimitedGreen;
using Random = UnityEngine.Random;

namespace TestFiles
{
    [RequireComponent(typeof(PlantRenderer))]
    public class TestB : MonoBehaviour
    {
        private Plant _plant;
        private PlantRenderer _plantRenderer;

        public float treeBranchDownPower;
        public int randomSeed;
        public float e;
        
        private void Awake()
        {
            (Vector3, Vector3) phytomerTopologyFunc(int axisOrder, Vector3 prePosition, Vector3 preDirection, float length)
            {
                if (axisOrder == 1)
                {
                    //(NewPosition, NewDirection)
                    var newPosition = prePosition + preDirection * length;
                    var newDirection = preDirection;
                    return (newPosition, newDirection);
                }
                else
                {
                    var newPosition = prePosition + preDirection * length;
                    var newDirection = preDirection+Random.Range(0f,0.1f) * new Vector3(Random.value,Random.value,Random.value).normalized;
                    return (newPosition, newDirection);
                }
            }

            Vector3 axisTopologyFunc(int axisOrder, Vector3 preDirection, Vector3 VerticleDirectionAfterPhyllotaxisRotation)
            {
                return (VerticleDirectionAfterPhyllotaxisRotation - preDirection * treeBranchDownPower).normalized; // 修改了这里的对preDirection的操作
            }
            
            var p1 = new Phytomer(1,new[]
            {
                new Phyllotaxis(90,true,BeerOrgan.Bud,1),
                new Phyllotaxis(90,true,BeerOrgan.Bud,1)
            });
            var p2 = new Phytomer(0, new[]
            {
                new Phyllotaxis(180, true, BeerOrgan.None),
                new Phyllotaxis(180, true, BeerOrgan.None)
            });
            
            // 自动机
            var Q1 = new InAutomaton(new[] { int.MaxValue }, new[,] { { 0.0f } },new[]{p1});
            var Q2 = new InAutomaton(new[] {int.MaxValue}, new[,] { {0.0f} }, new[] { p2});
            var automaton =
                new DualScaleAutomaton(new[] { int.MaxValue, int.MaxValue }, new float[,] { { 1.0f,0.0f}, { 0f, 0f } },new[]{Q1,Q2});
            
            // 芽
            var bud1 = new Bud(
                rhythmRatio: new[] { true },
                randomRatio: (_) => 0.9f,
                viabilityRatio: (_) => 1f,
                branchingIntensity: (_, _) => 1f,
                lightRatio: (_) => 1f
            );
            var bud2 = new Bud(
                rhythmRatio: new[] { true }, // 注意：这里的节律设定
                randomRatio: (_) => 0.9f, // 注意：这里的随机比
                viabilityRatio: (a) => (a==13? 0f: 0.95f), // 注意：这里的生存随机比
                branchingIntensity: (_, _) => 1f,
                lightRatio: (_) => 1f
            );
            
            // 植物
            _plant = new Plant(
                randomSeed: randomSeed,
                maxPhysiologicalAge: 2,
                initialBiomass: 0.3f,
                startDireciton: Vector3.up, 
                waterUseEfficiency: 1f, // 水利用率 r 
                projectionArea: 3f, //投影面积Sp
                extinctionCoefficient: 1f, // 消光系数
                leafAllometryE: 1f, //叶子厚度 e 
            
                leafSourceValidCycles:5, // 叶 - 源 - 有效周期 ※※※※※※※※※
                leafSinkValidCycles: 5, // 叶 - 汇 - 有效周期 ※※※※※※※※※
                leafSinkFunction: (phi, age) => { return 1.0f; }, // 叶 - 汇 - 函数 ※※※※※※※※※
            
                phytomerValidcycles: 8, // 叶元 - 汇 - 有效周期 ※※※※※※※※※
                phytomerSinkFunction: (phi, age) => phi==1 ? (0.25f + 0.75f * (age != 1 ? 1 : 0)) : (0.05f + 0.05f * (age != 1 ? 1 : 0)), // 叶元 - 汇 - 函数 ※※※※※※※※※
                phytomerAllometryDatas: new[] { (2f, 0f), (14f, 0f) }, // 叶元 - 异速数据 
            
                phytomerTopologyFunc: phytomerTopologyFunc,
                axisTopologyFunc: axisTopologyFunc,
                dualScaleAutomaton: automaton,
                buds: new Bud[]{bud1,bud2}
            );
            _plantRenderer = GetComponent<PlantRenderer>();
        }

        public void Growth(float e)
        {
            _plant.Growth(e);
            _plantRenderer.Render(_plant);
        }
    }
}