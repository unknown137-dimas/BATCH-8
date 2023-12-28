public delegate void MySubscriber(string name, string message);
class Youtuber
{
	private event MySubscriber mySub;
	string name;
	public Youtuber(string name) => this.name = name;
	public void UploadVideo(string title)
	{
		Console.WriteLine($"Uploading {title} video...");
		Console.WriteLine($"{title} uploaded...");
		SendNotification(title);
	}
	public void SendNotification(string message) => mySub(this.name, message);
	public bool AddSubscriber(MySubscriber sub)
	{
		if(mySub is null || !mySub.GetInvocationList().Contains(sub))
		{
			mySub += sub;
			return true;
		}
		return false;
	}
}