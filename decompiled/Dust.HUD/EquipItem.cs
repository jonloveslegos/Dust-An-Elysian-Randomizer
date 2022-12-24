namespace Dust.HUD
{
	public class EquipItem
	{
		protected string name;

		protected string description;

		protected string statsInfo;

		protected int value;

		protected int flag;

		protected int heal;

		protected int regen;

		protected int luck;

		protected int attackAdd;

		protected float attackMult;

		protected int defenseAdd;

		protected float defenseMult;

		protected float fidgetMult;

		private int[] materialReq = new int[6];

		private byte[] materialReqAmt = new byte[6];

		public string Name => this.name;

		public string Description => this.description;

		public string StatInfo => this.statsInfo;

		public int Value => this.value;

		public int Flag => this.flag;

		public int Heal => this.heal;

		public int Regen => this.regen;

		public int Luck => this.luck;

		public int AttackAdd => this.attackAdd;

		public float AttackMult => this.attackMult;

		public int DefenseAdd => this.defenseAdd;

		public float DefenseMult => this.defenseMult;

		public float FidgetMult => this.fidgetMult;

		public int[] MaterialReq => this.materialReq;

		public byte[] MaterialReqAmt => this.materialReqAmt;

		public EquipItem(string _name, string _desc, string _stats, int _value, int _flag, int _hpGain, int _regen, int _luck, int _augAdd, float _augMult, int _armAdd, float _armMult, float _fidget, int[] matReq, byte[] matReqAmt)
		{
			this.name = _name;
			this.description = _desc;
			this.statsInfo = _stats;
			this.value = _value;
			this.flag = _flag;
			this.heal = _hpGain;
			this.regen = _regen;
			this.luck = _luck;
			this.attackAdd = _augAdd;
			this.attackMult = _augMult;
			this.defenseAdd = _armAdd;
			this.defenseMult = _armMult;
			this.fidgetMult = _fidget;
			for (int i = 0; i < this.materialReq.Length; i++)
			{
				this.materialReq[i] = matReq[i];
				this.materialReqAmt[i] = matReqAmt[i];
			}
		}
	}
}
