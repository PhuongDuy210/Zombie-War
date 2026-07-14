public class PopupPage
{
    public int PageNumber { get; private set; }
    public string Content { get; private set; }

    public PopupPage(int pageNumber, string content)
    {
        PageNumber = pageNumber;
        Content = content;
    }
}
