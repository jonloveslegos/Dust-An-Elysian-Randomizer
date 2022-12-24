namespace Dust.Dialogue
{
	public class ResponseMaster
	{
		public string responseKey;

		public byte responseState;

		public ResponseMaster(string _responseKey)
		{
			this.responseKey = _responseKey;
			this.responseState = 2;
		}
	}
}
