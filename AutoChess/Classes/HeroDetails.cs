using System.Runtime.Serialization;

[DataContract]
public class HeroDetails : IHeroDetails
{
	[DataMember]
	public PieceTypes HeroType {get;}
	[DataMember]
	public double Hp {get;}
	[DataMember]
	public double Attack {get;}
	[DataMember]
	public double Armor {get;}
	[DataMember]
	public int AttackRange {get;}
	public HeroDetails(PieceTypes heroType, double hp, double attack, double armor, int attackRange)
	{
		HeroType = heroType;
		Hp = hp;
		Attack = attack;
		Armor = armor;
		AttackRange = attackRange;
	}

	public override bool Equals(object? obj)
    {
        if(obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        
        var other = obj as HeroDetails;
        if(ReferenceEquals(other, null))
            return false;
        if(ReferenceEquals(this, other))
            return true;
        return HeroType == other.HeroType && 
			Hp == other.Hp && 
			Attack == other.Attack && 
			Armor == other.Armor && 
			AttackRange == other.AttackRange;
    }

    public override int GetHashCode() =>  HeroType.GetHashCode() + Hp.GetHashCode() + Attack.GetHashCode() + Armor.GetHashCode() + AttackRange.GetHashCode();
	public static bool operator ==(HeroDetails self, HeroDetails other) => self.Equals(other);
    public static bool operator !=(HeroDetails self, HeroDetails other) => !self.Equals(other);
}