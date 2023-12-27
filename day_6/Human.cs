class Human
{
	private string _name;
	private DayOfWeek _day;
	private int _date;
	private Month _month;
	private int _year;
	public Human(string name, DayOfWeek day, int date, Month month, int year)
	
	{
		_name = name;
		_day = day;
		_date = date;
		_month = month;
		_year = year;
	}
	public (DayOfWeek, int, Month, int) GetDOB() => (_day, _date, _month, _year);
	public string GetName() => _name;
}