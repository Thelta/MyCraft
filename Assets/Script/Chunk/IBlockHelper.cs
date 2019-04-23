public interface IBlockHelper
{
    BlockType GetBlock(int x, int y, int z);
    void SetBlock(int x, int y, int z, BlockType block);
}
