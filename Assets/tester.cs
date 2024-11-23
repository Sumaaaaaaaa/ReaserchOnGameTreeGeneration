using System;
using UnityEngine;
using UnlimitedGreen;
public class tester: MonoBehaviour
{
    private Plant _plant;

    private void Start()
    {
        UnlimitedGreen.UnlimitedGreen.Test();
    }

    private void Awake()
    {
        (Vector3,Vector3) phytomerTopologyFunc(int order,Vector3 prePosition,Vector3 preDirection,float length)
        {
            if (order == 1)
            {
                // 直接往上面长
                var newPosition = prePosition + preDirection * length;

                return (newPosition, new Vector3(preDirection.x - 0.1f,preDirection.y,preDirection.z));
            }
            else if (order == 2)
            {
                // 直接继续往旁边长
                var newPosition = prePosition + preDirection * length;
                return (newPosition, preDirection);
            }
            Debug.LogError("出现了即不是order1也不是order2的情况。");
            
            return (prePosition, preDirection);
            //新位置，新方向
        }

        Vector3 axisTopologyFunc(int order, Vector3 preDirection, Vector3 verticleDirectionAfterPhyllotaxisRotation)
        {
            
            if (order != 2)
            {
                Debug.LogError("出现了非order为2的axisTopologyFunc，不应该出现这个情况");
            }

            return verticleDirectionAfterPhyllotaxisRotation;
        }
        var dualScaleAutomaton = new DualScaleAutomaton(
            new[] { 99/*, 99*/ },
            new float[,] { { 1f/*, 0f*/ }/*, { 0f, 1f }*/ },
            new[]
            {
                new InAutomaton(new[] { 99 },
                    new float[,] { { 1.0f } },
                    new[]
                    {
                        new Phytomer(0, new[] { new Phyllotaxis(90f, true, BeerOrgan.None, 1) })
                    })/*,
                new InAutomaton(new[] { 99 },new float[,] { { 1.0f } },new Phytomer[]
                {
                    new Phytomer(0,new []{new Phyllotaxis(180,true,BeerOrgan.None,0)})
                })*/
            }
        );
        print("before intial plant");
        _plant = new Plant(
            randomSeed: 0,
            maxPhysiologicalAge: 1,
            initialBiomass: 10,
            startDireciton: Vector3.up, 
            waterUseEfficiency: 0.5f, // 水利用率 r 
            projectionArea: 0.5f, //投影面积Sp
            extinctionCoefficient: 0.5f, // 消光系数
            leafAllometryE: 0.2f, //叶子厚度 e 
            leafSourceValidCycles: 3, // 叶 - 源 - 有效周期
            leafSinkValidCycles: 2, // 叶 - 汇 - 有效周期
            leafSinkFunction: (phi, age) => { return 1.0f; }, // 叶 - 汇 - 函数
            phytomerValidcycles: 2, // 叶元 - 汇 - 有效周期
            phytomerSinkFunction: (phi, age) => { return 1.0f; }, // 叶元 - 汇 - 函数
            phytomerAllometryDatas: new[] { (1f, 0f), /*(1f, 0f)*/ }, // 叶元 - 异速数据
            phytomerTopologyFunc: phytomerTopologyFunc,
            axisTopologyFunc: axisTopologyFunc,
            dualScaleAutomaton: dualScaleAutomaton,
            buds: new Bud[]
            {
                new Bud(new[] { true },
                    (_) => { return 1.0f; },
                    (_) => { return 1.0f; },
                    (_, _, _) => { return 1.0f; },
                    (_) => { return 1.0f; })/*,
                new Bud(new[] { true },
                    (_) => { return 1.0f; },
                    (_) => { return 1.0f; },
                    (_, _, _) => { return 1.0f; },
                    (_) => { return 1.0f; })*/
            }
        );
    }

    private void OnDrawGizmos()
    {
        _plant?.GizmosDraw();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _plant.Growth(1.0f);
        }
    }
}
