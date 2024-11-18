using UnityEngine;

namespace UnlimitedGreen
{
    public class UnlimitedGreen
    {
        public UnlimitedGreen() // 用于测试
        {
            Debug.Log("1");
            var a = new (float, float)[2];

        }
        private int _age = 0;
        private void PrimaryGrowth()
        {
            
        }

        public void Growth()
        {
            _age++;
        }
    }
}