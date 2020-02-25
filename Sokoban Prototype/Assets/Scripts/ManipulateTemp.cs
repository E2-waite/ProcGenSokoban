public class ManipulateTemp
{
    public int[,] Rotate(int[,] template)
    {
        // Rotate template 90 degrees clockwise
        int[,] rotated_template = new int[5, 5];
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                rotated_template[i, j] = template[5 - j - 1, i];
            }
        }
        return rotated_template;
    }

    public int[,] Flip(int[,] template)
    {
        // Flip template horizontally
        int[,] flipped_template = new int[5, 5];
        for (int y = 0; y < 5; y++)
        {
            int new_x = 4;
            for (int x = 0; x < 5; x++)
            {
                flipped_template[x, y] = template[new_x, y];
                new_x--;
            }
        }
        return flipped_template;
    }
}
