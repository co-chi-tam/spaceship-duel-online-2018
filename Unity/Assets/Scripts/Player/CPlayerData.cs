using System;

[Serializable]
	public class CPlayerData {
		public string id;
		public string name;
		public string formation;
		public int turnIndex;

		public CPlayerData()
		{
			this.id = string.Empty;
			this.name = string.Empty;
			this.formation = string.Empty;
			this.turnIndex = -1;
		}

		public CPlayerData(CPlayerData value)
		{
			this.id = value.id;
			this.name = value.name;
			this.formation = value.formation;
			this.turnIndex = value.turnIndex;
		}
	}
