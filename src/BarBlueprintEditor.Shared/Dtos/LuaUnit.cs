using System.Text.Json.Serialization;

namespace BarBlueprintEditor.Shared.Dtos;

/// <summary>
/// https://github.com/beyond-all-reason/Beyond-All-Reason/blob/master/units/ArmBuildings/LandDefenceOffence/armflak.lua
/// </summary>
public class LuaRoot
{
    [JsonExtensionData]
    public Dictionary<string, LuaUnit> Units { get; set; }
}

public class LuaUnit
{
    [JsonPropertyName("unitInfo")]
    public WebUnitDefinition UnitInfo { get; set; }
    
    [JsonPropertyName("airsightdistance")]
    public float Airsightdistance { get; set; }

    [JsonPropertyName("buildangle")]
    public float Buildangle { get; set; }

    [JsonPropertyName("buildpic")]
    public string Buildpic { get; set; }

    [JsonPropertyName("buildtime")]
    public float Buildtime { get; set; }

    [JsonPropertyName("canrepeat")]
    public bool Canrepeat { get; set; }

    [JsonPropertyName("collisionvolumeoffsets")]
    public string Collisionvolumeoffsets { get; set; }

    [JsonPropertyName("collisionvolumescales")]
    public string Collisionvolumescales { get; set; }

    [JsonPropertyName("collisionvolumetype")]
    public string Collisionvolumetype { get; set; }

    [JsonPropertyName("corpse")]
    public string Corpse { get; set; }

    [JsonPropertyName("energycost")]
    public float Energycost { get; set; }

    [JsonPropertyName("explodeas")]
    public string Explodeas { get; set; }

    [JsonPropertyName("footprintx")]
    public float Footprintx { get; set; }

    [JsonPropertyName("footprintz")]
    public float Footprintz { get; set; }

    [JsonPropertyName("health")]
    public float Health { get; set; }

    [JsonPropertyName("maxacc")]
    public float Maxacc { get; set; }

    [JsonPropertyName("maxdec")]
    public float Maxdec { get; set; }

    [JsonPropertyName("maxslope")]
    public float Maxslope { get; set; }

    [JsonPropertyName("maxwaterdepth")]
    public float Maxwaterdepth { get; set; }

    [JsonPropertyName("metalcost")]
    public float Metalcost { get; set; }

    [JsonPropertyName("nochasecategory")]
    public string Nochasecategory { get; set; }

    [JsonPropertyName("objectname")]
    public string Objectname { get; set; }

    [JsonPropertyName("script")]
    public string Script { get; set; }

    [JsonPropertyName("seismicsignature")]
    public float Seismicsignature { get; set; }

    [JsonPropertyName("selfdestructas")]
    public string Selfdestructas { get; set; }

    [JsonPropertyName("sightdistance")]
    public float Sightdistance { get; set; }

    [JsonPropertyName("yardmap")]
    public string Yardmap { get; set; }

    [JsonPropertyName("customparams")]
    public Customparams Customparams { get; set; }

    [JsonPropertyName("featuredefs")]
    public Featuredefs Featuredefs { get; set; }

    [JsonPropertyName("sfxtypes")]
    public Sfxtypes Sfxtypes { get; set; }

    [JsonPropertyName("sounds")]
    public Sounds Sounds { get; set; }

    [JsonPropertyName("weapondefs")]
    public Weapondefs Weapondefs { get; set; }

    //[JsonPropertyName("weapons")]
    //public Weapons Weapons { get; set; }
}

public class ArmflakGun
{
    [JsonPropertyName("accuracy")]
    public float Accuracy { get; set; }

    [JsonPropertyName("areaofeffect")]
    public float Areaofeffect { get; set; }

    [JsonPropertyName("avoidfeature")]
    public bool Avoidfeature { get; set; }

    [JsonPropertyName("avoidfriendly")]
    public bool Avoidfriendly { get; set; }

    [JsonPropertyName("burnblow")]
    public bool Burnblow { get; set; }

    [JsonPropertyName("canattackground")]
    public bool Canattackground { get; set; }

    [JsonPropertyName("cegtag")]
    public string Cegtag { get; set; }

    [JsonPropertyName("collidefriendly")]
    public bool Collidefriendly { get; set; }

    [JsonPropertyName("craterareaofeffect")]
    public float Craterareaofeffect { get; set; }

    [JsonPropertyName("craterboost")]
    public float Craterboost { get; set; }

    [JsonPropertyName("cratermult")]
    public float Cratermult { get; set; }

    [JsonPropertyName("cylindertargeting")]
    public float Cylindertargeting { get; set; }

    [JsonPropertyName("edgeeffectiveness")]
    public float Edgeeffectiveness { get; set; }

    [JsonPropertyName("explosiongenerator")]
    public string Explosiongenerator { get; set; }

    [JsonPropertyName("gravityaffected")]
    public string Gravityaffected { get; set; }

    [JsonPropertyName("impulsefactor")]
    public float Impulsefactor { get; set; }

    [JsonPropertyName("mygravity")]
    public double Mygravity { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("noselfdamage")]
    public bool Noselfdamage { get; set; }

    [JsonPropertyName("predictboost")]
    public float Predictboost { get; set; }

    [JsonPropertyName("range")]
    public float Range { get; set; }

    [JsonPropertyName("reloadtime")]
    public double Reloadtime { get; set; }

    [JsonPropertyName("smoketrail")]
    public bool Smoketrail { get; set; }

    [JsonPropertyName("soundhit")]
    public string Soundhit { get; set; }

    [JsonPropertyName("soundhitvolume")]
    public double Soundhitvolume { get; set; }

    [JsonPropertyName("soundhitwet")]
    public string Soundhitwet { get; set; }

    [JsonPropertyName("soundstart")]
    public string Soundstart { get; set; }

    [JsonPropertyName("soundstartvolume")]
    public float Soundstartvolume { get; set; }

    [JsonPropertyName("stages")]
    public float Stages { get; set; }

    [JsonPropertyName("turret")]
    public bool Turret { get; set; }

    [JsonPropertyName("weapontimer")]
    public float Weapontimer { get; set; }

    [JsonPropertyName("weapontype")]
    public string Weapontype { get; set; }

    [JsonPropertyName("weaponvelocity")]
    public float Weaponvelocity { get; set; }

    [JsonPropertyName("damage")]
    public Damage Damage { get; set; }

    [JsonPropertyName("rgbcolor")]
    public Rgbcolor Rgbcolor { get; set; }
}

public class Cant
{
    [JsonPropertyName("[1]")]
    public string _1 { get; set; }
}

public class Count
{
    [JsonPropertyName("[1]")]
    public string _1 { get; set; }

    [JsonPropertyName("[2]")]
    public string _2 { get; set; }

    [JsonPropertyName("[3]")]
    public string _3 { get; set; }

    [JsonPropertyName("[4]")]
    public string _4 { get; set; }

    [JsonPropertyName("[5]")]
    public string _5 { get; set; }

    [JsonPropertyName("[6]")]
    public string _6 { get; set; }
}

public class Customparams
{
    [JsonPropertyName("buildinggrounddecaldecayspeed")]
    public float Buildinggrounddecaldecayspeed { get; set; }

    [JsonPropertyName("buildinggrounddecalsizex")]
    public float Buildinggrounddecalsizex { get; set; }

    [JsonPropertyName("buildinggrounddecalsizey")]
    public float Buildinggrounddecalsizey { get; set; }

    [JsonPropertyName("buildinggrounddecaltype")]
    public string Buildinggrounddecaltype { get; set; }

    [JsonPropertyName("model_author")]
    public string ModelAuthor { get; set; }

    [JsonPropertyName("normaltex")]
    public string Normaltex { get; set; }

    [JsonPropertyName("removewait")]
    public bool Removewait { get; set; }

    [JsonPropertyName("subfolder")]
    public string Subfolder { get; set; }

    [JsonPropertyName("techlevel")]
    public float Techlevel { get; set; }

    [JsonPropertyName("unitgroup")]
    public string Unitgroup { get; set; }

    [JsonPropertyName("usebuildinggrounddecal")]
    public bool Usebuildinggrounddecal { get; set; }
}

public class Damage
{
    [JsonPropertyName("vtol")]
    public float Vtol { get; set; }
}

public class Dead
{
    [JsonPropertyName("blocking")]
    public bool Blocking { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("collisionvolumeoffsets")]
    public string Collisionvolumeoffsets { get; set; }

    [JsonPropertyName("collisionvolumescales")]
    public string Collisionvolumescales { get; set; }

    [JsonPropertyName("collisionvolumetype")]
    public string Collisionvolumetype { get; set; }

    [JsonPropertyName("damage")]
    public float Damage { get; set; }

    [JsonPropertyName("featuredead")]
    public string Featuredead { get; set; }

    [JsonPropertyName("footprintx")]
    public float Footprintx { get; set; }

    [JsonPropertyName("footprintz")]
    public float Footprintz { get; set; }

    [JsonPropertyName("height")]
    public float Height { get; set; }

    [JsonPropertyName("metal")]
    public float Metal { get; set; }

    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("reclaimable")]
    public bool Reclaimable { get; set; }
}

public class Explosiongenerators
{
    [JsonPropertyName("[1]")]
    public string _1 { get; set; }
}

public class Featuredefs
{
    [JsonPropertyName("dead")]
    public Dead Dead { get; set; }

    [JsonPropertyName("heap")]
    public Heap Heap { get; set; }
}

public class Heap
{
    [JsonPropertyName("blocking")]
    public bool Blocking { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("collisionvolumescales")]
    public string Collisionvolumescales { get; set; }

    [JsonPropertyName("collisionvolumetype")]
    public string Collisionvolumetype { get; set; }

    [JsonPropertyName("damage")]
    public float Damage { get; set; }

    [JsonPropertyName("footprintx")]
    public float Footprintx { get; set; }

    [JsonPropertyName("footprintz")]
    public float Footprintz { get; set; }

    [JsonPropertyName("height")]
    public float Height { get; set; }

    [JsonPropertyName("metal")]
    public float Metal { get; set; }

    [JsonPropertyName("object")]
    public string Object { get; set; }

    [JsonPropertyName("reclaimable")]
    public bool Reclaimable { get; set; }

    [JsonPropertyName("resurrectable")]
    public float Resurrectable { get; set; }
}

public class Ok
{
    [JsonPropertyName("[1]")]
    public string _1 { get; set; }
}

public class Pieceexplosiongenerators
{
    [JsonPropertyName("[1]")]
    public string _1 { get; set; }

    [JsonPropertyName("[2]")]
    public string _2 { get; set; }

    [JsonPropertyName("[3]")]
    public string _3 { get; set; }
}

public class Rgbcolor
{
    [JsonPropertyName("[1]")]
    public float _1 { get; set; }

    [JsonPropertyName("[2]")]
    public double _2 { get; set; }

    [JsonPropertyName("[3]")]
    public double _3 { get; set; }
}

public class Select
{
    [JsonPropertyName("[1]")]
    public string _1 { get; set; }
}

public class Sfxtypes
{
    [JsonPropertyName("explosiongenerators")]
    public Explosiongenerators Explosiongenerators { get; set; }

    [JsonPropertyName("pieceexplosiongenerators")]
    public Pieceexplosiongenerators Pieceexplosiongenerators { get; set; }
}

public class Sounds
{
    [JsonPropertyName("canceldestruct")]
    public string Canceldestruct { get; set; }

    [JsonPropertyName("cloak")]
    public string Cloak { get; set; }

    [JsonPropertyName("uncloak")]
    public string Uncloak { get; set; }

    [JsonPropertyName("underattack")]
    public string Underattack { get; set; }

    [JsonPropertyName("cant")]
    public Cant Cant { get; set; }

    [JsonPropertyName("count")]
    public Count Count { get; set; }

    [JsonPropertyName("ok")]
    public Ok Ok { get; set; }

    [JsonPropertyName("select")]
    public Select Select { get; set; }
}

public class Weapondefs
{
    [JsonPropertyName("armflak_gun")]
    public ArmflakGun ArmflakGun { get; set; }
}

//public class Weapons
//{
//    [JsonPropertyName("[1]")]
//    public 1 _1 { get; set; }
//}

public class Weapon
{
    [JsonPropertyName("badtargetcategory")]
    public string Badtargetcategory { get; set; }

    [JsonPropertyName("def")]
    public string Def { get; set; }

    [JsonPropertyName("onlytargetcategory")]
    public string Onlytargetcategory { get; set; }
}

