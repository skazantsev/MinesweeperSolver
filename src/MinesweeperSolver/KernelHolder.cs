using Ninject;

namespace MinesweeperSolver
{
    public static class KernelHolder
    {
        public static IKernel Kernel { get; private set; }

        public static void Initialize(IKernel kernel)
        {
            Kernel = kernel;
        }
    }
}
