namespace RestaurantApi.Models;

public class PageResult<T>
{
    public List<T> Items { get; set; }
    public int TotalPages { get; set; }
    public int ItemFrom { get; set; }
    public int ItemTo { get; set; }
    public int TotalItemsCount { get; set; }

    public PageResult(List<T> items, int totalCount, int pageSize, int pageNumber)
    {
        Items = items;
        TotalItemsCount = totalCount;
        ItemFrom = pageSize * (pageNumber - 1) + 1;
        ItemTo = ItemFrom + items.Count - 1;
        TotalPages = (int) Math.Ceiling((double) totalCount / (double) pageSize);
    }
}