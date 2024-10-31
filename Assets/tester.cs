using Model;
using Model.Automaton;
using Model.Bud;
using UnityEngine;

namespace DefaultNamespace
{
    public class tester : MonoBehaviour
    {
        [SerializeField] private int randomseed;

        private void Awake()
        {
            var A = new Phytomer() { Name = "A" };
            var B = new Phytomer() { Name = "B" };
            var C = new Phytomer() { Name = "C" };
            var D = new Phytomer() { Name = "D" };
            var E = new Phytomer() { Name = "E" };
            var F = new Phytomer() { Name = "F" };
            var G = new Phytomer() { Name = "G" };

            var X = new InAutomaton
            (
                new Phytomer[] { A, B },
                new [] { 1, 0 },
                new float[,] { { 0, 1 }, { 1, 0 } },
                0,
                true
            );
            var Y = new InAutomaton(
                new [] { C, D, E },
                new [] { 2, 0, 0 },
                new [,] { { 0, 0.25f, 0.75f }, { 0, 0, 0 }, { 0, 0, 0 } },
                0
            );
            var Z = new InAutomaton(
                new [] { F, G },
                new[] { 0, 0 },
                new [,] { { 0, 1.0f }, { 0, 0 } },
                0
            );

            var Final = new OutAutomaton(
                new[] { X, Y, Z },
                new[] { 0, 0, 0 },
                new float[,] { { 0, 0.5f, 0.5f }, { 0, 0, 0 }, { 0, 0, 0 } },
                0
            );

            var bud = new Bud(
                (_) => 1.0f,
                (_) => 1.0f,
                new[] { true },
                (_, _) => 1.0f,
                (_) => 1.0f,
                Final,
                () => 4
            );

            var builtinBud = new Builtin_Bud(bud,A,randomseed);
            for (var i = 0; i < 10; i++)
            {
                var isActiveBud = builtinBud.Expansion(out var d);
                var returnString = "";
                foreach (var dd in d)
                {
                    returnString += dd?.Name;
                    returnString += "/";
                }
                print($"{isActiveBud}...{d.Length}...{returnString}");
            }
        }
    }
}