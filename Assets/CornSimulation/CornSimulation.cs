using System;
using UnityEngine;
using UnlimitedGreen;
using Random = UnityEngine.Random;

namespace CornSimulation
{
    internal class AutomatonFunc
    {
        private System.Random _random;
        private Plant _plant;
        public int Te { get; }
        public int Tt { get; }

        public AutomatonFunc(int randomSeed,Plant plant)
        {
            _random = new System.Random(randomSeed);
            Te = _random.Next(9, 13);
            Tt = _random.Next(20, 23);
            _plant = plant;
        }

        public float Random()
        {
            return (float)_random.NextDouble();
        }
    }
    public class CornSimulation : MonoBehaviour
    {
        private static Func<int,int,float> BetaLaw(float a,float b,float T,float m)
        {
            var N = Mathf.Pow((a - 1) / (a + b - 2), a - 1) * Mathf.Pow((b - 1) / (a + b - 2), b - 1);
            return (_, age) =>
            {
                if (age > T)
                {
                    Debug.LogError("age > T ,is not allowed.");
                }
                return m * 1 / N * Mathf.Pow(age / T, a - 1) * Mathf.Pow(1 - age / T, b - 1);
            };
        }
        private static Func<int, float> BetaLaw2(float a, float b, float T, float m)
        {
            var N = Mathf.Pow((a - 1) / (a + b - 2), a - 1) * Mathf.Pow((b - 1) / (a + b - 2), b - 1);
            return (age) =>
            {
                if (age > T)
                {
                    Debug.LogError("age > T ,is not allowed.");
                }
                return m * 1 / N * Mathf.Pow(age / T, a - 1) * Mathf.Pow(1 - age / T, b - 1);
            };
        }
        public int randomSeed = 0;
        private Plant _plant;

        private AutomatonFunc _af;

        private PlantRenderer _plantRenderer;
        private void Awake()
        {
            _af = new AutomatonFunc(randomSeed + 1,_plant);
            _plantRenderer = gameObject.GetComponent<PlantRenderer>();

            // 叶元设计
            var pDec = new Phytomer(0.1f,
                new[] { new Phyllotaxis(180, true, BeerOrgan.None, 0), new Phyllotaxis(180, true, BeerOrgan.None, 0) });
            var pAlt = new Phytomer(0.1f, new[] { new Phyllotaxis(180, true, BeerOrgan.None, 0) });
            var pDecCob = new Phytomer(0.1f,
                new[] { new Phyllotaxis(180, true, BeerOrgan.None, 0), new Phyllotaxis(180, true, BeerOrgan.Fruit) });
            var pAltCob = new Phytomer(0.1f, new[] { new Phyllotaxis(180, true, BeerOrgan.Fruit, 0) });
            var pT = new Phytomer(0, new[] { new Phyllotaxis(180, false, BeerOrgan.Flower) });

            // 自动机
            var A1 = new InAutomaton(new[] { 0, 0 }, new float[,] { { 0.5f, 0.5f }, { 0.5f, 0.5f } },
                new[] { pAlt, pDec });
            var A2 = new InAutomaton(new []{0,0,0},new float[,]{{0,0.5f,0.5f},{0,0,0},{0,0,0}},new []{pAlt,pDecCob,pAltCob});
            var A3 = new InAutomaton(new[] { 0 }, new float[,] { { 0 } }, new[] { pT });
            var automaton = new DualScaleAutomaton(new[] { 9, 1, 9, 0 },
                new float[,] { { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 }, { 0, 0, 0, 0 } },
                new[] { A1, A2, A1, A3 });
            // TODO: 现在的重复次数是固定的可以通过Te/Tt进行随机性的创建
            // TODO: 但是现在这两个数值展现的是总次数，而不是分在A2和A2_1中的分批次数！
            
            // 芽
            var bud = new Bud(
                new[] { true },
                (_) => 1,
                (_) => 1,
                (_, _) => 1,
                (_) => 1
            );

        _plant = new Plant(
                randomSeed: randomSeed,
                maxPhysiologicalAge: 4,
                initialBiomass: 1.1f,

                // 启动时拓扑信息
                startDireciton: Vector3.up,

                // 源计算相关参数 
                waterUseEfficiency: 0.056f, /* g MJ-1 单位可能不正确*/
                projectionArea: 0.1200f, /* cm2 单位可能不正确*/
                extinctionCoefficient: 0.1f, /*论文中没有记述*/

                // 叶子
                leafAllometryE: 0.024f, /*单位可能不正确*/
                leafSourceValidCycles: 12, /*可能需要调整*/
                leafSinkValidCycles: 12, /*可能需要调整*/
                leafSinkFunction: BetaLaw(2.7f, 3.8f, 12f, 1f), // 注意检查可能出现的BetaLaw的错误

                // 叶元数据
                phytomerValidcycles: 20,
                phytomerSinkFunction: BetaLaw(4.1f, 4.1f, 20, 1.5f),
                phytomerAllometryDatas: new []{(8.4f,0f),(8.4f,0f),(8.4f,0f),(8.4f,0f)}, //TODO: 必须要进行经验性调整
                phytomerTopologyFunc: (_, prePosition, preDirection, length) =>
                {
                    var newPosition = prePosition + preDirection * length;
                    var newDirection = Vector3.up + new Vector3((_af.Random()-0.5f)*0.25f,0,(_af.Random()-0.5f)*0.25f);
                    newDirection = newDirection.normalized;
                    return (newPosition, newDirection);
                },
                axisTopologyFunc: (_, _, _) => Vector3.zero, //忽略的

                // 自动机
                dualScaleAutomaton:automaton,
                buds: new[]
                {bud,bud,bud,bud},
                flower: new Flower(2, (_) => 20),
                fruit: new Fruit(30, BetaLaw2(7.3f, 3.8f, 30f, 223.85f))
            );
        }

        private void OnDrawGizmos()
        {
            if (_plant is not null) _plantRenderer.GizmosDraw(_plant);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _plant.Growth(1.0f);
                _plantRenderer.Render(_plant);
            }
        }
    }
}