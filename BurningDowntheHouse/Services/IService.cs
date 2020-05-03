using System.Threading.Tasks;

namespace BurningDowntheHouse.Services
{
	public interface IService
	{
		Task Initialize();
		Task Start();
		Task Dispose();
	}
}
