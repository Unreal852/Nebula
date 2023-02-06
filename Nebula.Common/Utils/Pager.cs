namespace Nebula.Common.Utils;

public class Pager<T>
{
    public Pager(IList<T> list, int pageSize = 20)
    {
        Source = list;
        PageSize = pageSize;
        Refresh();
    }

    public int      PageSize    { get; set; }
    public int      CurrentPage { get; private set; } = 1;
    public int      MaxPage     { get; private set; }
    public IList<T> Source      { get; }

    public IEnumerable<T> PageElements => Source.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
    public IEnumerable<T> this[int page] => Source.Skip((page      - 1) * PageSize).Take(PageSize);

    public IEnumerable<T> ApplyPaging(IEnumerable<T> enumerable) =>
            enumerable.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

    public int NextPage()
    {
        if (CurrentPage >= MaxPage)
            return CurrentPage;
        return ++CurrentPage;
    }

    public int PreviousPage()
    {
        if (CurrentPage <= 1)
            return CurrentPage;
        return --CurrentPage;
    }

    public void Refresh()
    {
        MaxPage = (int)Math.Ceiling((decimal)Source.Count / PageSize);
    }
}