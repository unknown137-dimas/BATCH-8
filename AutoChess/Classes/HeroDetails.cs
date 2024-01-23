using System.Runtime.Serialization;

[DataContract]
public class HeroDetails
{
	[DataMember]
	public PieceTypes HeroType;
	[DataMember]
	public double Hp;
	[DataMember]
	public double Attack;
	[DataMember]
	public double Armor;
	[DataMember]
	public int AttackRange;
	public HeroDetails(PieceTypes heroType, double hp, double attack, double armor, int attackRange)
	{
		HeroType = heroType;
		Hp = hp;
		Attack = attack;
		Armor = armor;
		AttackRange = attackRange;
	}
}