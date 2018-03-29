namespace DiscontMD.BusinessLogic.Presistense.MSSQL
{
    public class PagingArgs
    {
        public const int DefaultRowsPerPage = 10;

        public int RowsPerPage { get; set; }
        public string Sort { get; set; }
        public SortDir SortDir { get; set; }
        public int Page { get; set; }
    }
}