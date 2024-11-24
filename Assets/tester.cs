using System;
using UnityEngine;
using UnlimitedGreen;
public class tester: MonoBehaviour
{
    private Plant _plant;
    public int RandomSeed;
    private void Start()
    {
        UnlimitedGreen.UnlimitedGreen.Test();
    }

    private void Awake()
    {
        (Vector3, Vector3) phytomerTopologyFunc(int axisOrder, Vector3 prePosition, Vector3 preDirection, float length)
        {
            //(NewPosition, NewDirection)
            var newPosition = prePosition + preDirection * length;
            var newDirection = preDirection;
            return (newPosition, newDirection);
        }

        Vector3 axisTopologyFunc(int axisOrder, Vector3 preDirection, Vector3 VerticleDirectionAfterPhyllotaxisRotation)
        {
            return VerticleDirectionAfterPhyllotaxisRotation;
        }
        
        // 叶元
        var p1 = new Phytomer(0,new[]
        {
            new Phyllotaxis(90,true,BeerOrgan.Bud,1),
            new Phyllotaxis(90,true,BeerOrgan.Bud,1)
        });
        var p2 = new Phytomer(0, new[]
        {
            new Phyllotaxis(180, true, BeerOrgan.None),
            new Phyllotaxis(180, true, BeerOrgan.None)
        });
        var p3 = new Phytomer(0,new[]
        {
            new Phyllotaxis(180,true,BeerOrgan.None),
            new Phyllotaxis(180,true,BeerOrgan.Flower)
        });
        
        // 自动机
        var Q1 = new InAutomaton(new[] { int.MaxValue }, new[,] { { 0.0f } },new[]{p1});
        var Q2 = new InAutomaton(new[] { 0, 0 }, new[,] { {0.8f, 0.2f }, {0f, 0f } }, new[] { p2, p3 });
        var automaton =
            new DualScaleAutomaton(new[] { 0, int.MaxValue }, new float[,] { { 0.9f, 0.1f }, { 0f, 0f } },new[]{Q1,Q2});
        
        //芽
        var bud = new Bud(
            rhythmRatio: new[] { true},
            randomRatio: (_) => 1,
            viabilityRatio: (_) => 1f,
            branchingIntensity: (_, _) => 1f,
            lightRatio: (_) => 1f
        );
        
        _plant = new Plant(
            randomSeed: RandomSeed,
            maxPhysiologicalAge: 2,
            initialBiomass: 1,
            startDireciton: Vector3.up, 
            waterUseEfficiency: 1f, // 水利用率 r 
            projectionArea: 1f, //投影面积Sp
            extinctionCoefficient: 1f, // 消光系数
            leafAllometryE: 1f, //叶子厚度 e 
            
            leafSourceValidCycles: 3, // 叶 - 源 - 有效周期 ※※※※※※※※※
            leafSinkValidCycles: 2, // 叶 - 汇 - 有效周期 ※※※※※※※※※
            leafSinkFunction: (phi, age) => { return 1.0f; }, // 叶 - 汇 - 函数 ※※※※※※※※※
            
            phytomerValidcycles: 2, // 叶元 - 汇 - 有效周期 ※※※※※※※※※
            phytomerSinkFunction: (phi, age) => { return 1.0f; }, // 叶元 - 汇 - 函数 ※※※※※※※※※
            phytomerAllometryDatas: new[] { (1f, 0f), (1f, 0f) }, // 叶元 - 异速数据 
            
            phytomerTopologyFunc: phytomerTopologyFunc,
            axisTopologyFunc: axisTopologyFunc,
            dualScaleAutomaton: automaton,
            buds: new Bud[]{bud,bud},
            flower:new Flower(2,(_)=>1f)
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
