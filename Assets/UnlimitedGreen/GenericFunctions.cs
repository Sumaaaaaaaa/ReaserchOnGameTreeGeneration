using UnityEngine;

namespace UnlimitedGreen
{
    internal static class GenericFunctions
    {
        /// <summary>
        /// 通过植物个体的年龄和器官的出生日期计算器官的年龄
        /// </summary>
        /// <param name="plantAge"></param>
        /// <param name="birthCycle"></param>
        /// <returns></returns>
        internal static int CalculateAge(int plantAge, int birthCycle)
        {
            return plantAge - birthCycle + 1;
        }
        
        /// <summary>
        /// 通过前个方向和经过处理后的新方向计算新的副朝向向量。
        /// </summary>
        /// <param name="preDirection"></param>
        /// <param name="newDirection"></param>
        /// <param name="preSubDirection"></param>
        /// <returns></returns>
        internal static Vector3 NewSubDirection(Vector3 preDirection, Vector3 newDirection, 
            Vector3 preSubDirection)
        {
            Quaternion rotation = Quaternion.FromToRotation(preDirection, newDirection);
            Matrix4x4 transformationMatrix = Matrix4x4.Rotate(rotation);
            Vector3 transformedB = transformationMatrix.MultiplyPoint3x4(preSubDirection);
            return transformedB;
        }

        /// <summary>
        /// 通过朝向和副朝向，加以定义的叶轴旋转量计算得到垂直于叶元的叶轴旋转后垂直朝向
        /// </summary>
        /// <param name="phyllotaxisDirection"></param>
        /// <param name="direction"></param>
        /// <param name="subDirection"></param>
        /// <returns></returns>
        internal static Vector3 PhyllotaxisToVerticalDirection
            (float phyllotaxisDirection, Vector3 direction, Vector3 subDirection)
        {
            Quaternion rotation = Quaternion.AngleAxis(phyllotaxisDirection, direction);
            return rotation * subDirection;
        }
    }
}