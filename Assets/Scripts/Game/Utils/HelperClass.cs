
namespace Everest.PuzzleGame
{
   public static class HelperClass
    {
        static public void MergeSort(LeaderBoardUserData[] data, int left, int mid, int right)
        {
            var temp = new LeaderBoardUserData[data.Length];
            int i, eol, num, pos;
            eol = (mid - 1);
            pos = left;
            num = (right - left + 1);

            while ((left <= eol) && (mid <= right))
            {
                if (data[left].BestScore <= data[mid].BestScore)
                    temp[pos++] = data[left++];
                else
                    temp[pos++] = data[mid++];
            }
            while (left <= eol)
                temp[pos++] = data[left++];
            while (mid <= right)
                temp[pos++] = data[mid++];
            for (i = 0; i < num; i++)
            {
                data[right] = temp[right];
                right--;
            }
        }

        static public void Sort(LeaderBoardUserData[] data, int left, int right)
        {
            int mid;
            if (right > left)
            {
                mid = (right + left) / 2;
                Sort(data, left, mid);
                Sort(data, (mid + 1), right);
                MergeSort(data, left, (mid + 1), right);
            }
        }
    }
}
