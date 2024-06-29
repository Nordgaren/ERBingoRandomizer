using System;

namespace Project.Settings;
// Non configurable constants  
public static class Const
{
    // Paths  
    public const string ME2Path = $"{Config.ResourcesPath}/ME2";
    public const string BingoPath = $"{ME2Path}/bingo";
    public const string BingoRegulationPath = $"{BingoPath}/{RegulationName}";
    public const string ExeName = "eldenring.exe";
    public const string RegulationName = "regulation.bin";
    public const string ItemMsgBNDPath = "/msg/engus/item.msgbnd.dcx";
    public const string MenuMsgBNDPath = "/msg/engus/menu.msgbnd.dcx";
    // Params  
    public const string EquipParamWeaponName = "EquipParamWeapon.param";
    public const string EquipParamCustomWeaponName = "EquipParamCustomWeapon.param";
    public const string EquipParamGoodsName = "EquipParamGoods.param";
    public const string EquipParamAccessoryName = "EquipParamAccessory.param";
    public const string EquipParamProtectorName = "EquipParamProtector.param";
    public const string MagicName = "Magic.param";
    public const string CharaInitParamName = "CharaInitParam.param";
    public const string ItemLotParam_mapName = "ItemLotParam_map.param";
    public const string ItemLotParam_enemyName = "ItemLotParam_enemy.param";
    public const string ShopLineupParamName = "ShopLineupParam.param";
    public const string AtkParamPcName = "AtkParam_Pc.param";
    // FMGS  
    public const string WeaponNameName = "WeaponName.fmg";
    public const string ProtectorNameName = "ProtectorName.fmg";
    public const string GoodsNameName = "GoodsName.fmg";
    public const string AccessoryNameName = "AccessoryName.fmg";
    public const string GR_LineHelpName = "GR_LineHelp.fmg";
    public const string GR_MenuTextName = "GR_MenuText.fmg";
    // Gear Randomizer  
    public const int NoItem = -1;
    public const byte HelmType = 0;
    public const byte BodyType = 1;
    public const byte ArmType = 2;
    public const byte LegType = 3;
    public const ushort BowType = 51;
    public const ushort LightBowType = 50;
    public const ushort GreatbowType = 53;
    public const ushort CrossbowType = 55;
    public const ushort BallistaType = 56;
    public const ushort ArrowType = 81;
    public const ushort GreatArrowType = 83;
    public const ushort BoltType = 85;
    public const ushort BallistaBoltType = 86;
    public const byte SorceryType = 0;
    public const byte IncantationType = 1;
    public const byte GoodsSorceryType = 5;
    public const byte GoodsIncantationType = 16;
    public const byte GoodsSelfSorceryType = 17;
    public const byte GoodsSelfIncantationType = 18;
    public const ushort StaffType = 57;
    public const ushort SealType = 61;
    public static readonly string ExeDir = Environment.CurrentDirectory;
    // Level Randomizer  
    public const byte NumStats = 8;
    // Item Lot offsets
    public const int ItemLots = 8;
    public const int CategoriesStart = 8;
    public const int ChanceStart = 16;
    // Item Lot constants 
    public const int ItemLotGoodsCategory = 1;
    public const int ItemLotWeaponCategory = 2;
    public const int ItemLotCustomWeaponCategory = 6;
    // Shop Lineup constants
    public const byte ShopLineupWeaponCategory = 0;
    public const byte ShopLineupGoodsCategory = 3;
    //CharaInitParam Msg Bnd Entries  
    public static readonly int[] ChrInfoMapping = {
        297130, 297131, 297132, 297133, 297134, 297135, 297138, 297136, 297137, 297139,
    };
    // BHD5  
    public static class ArchiveKeys
    {
        public const string DATA0 = @"-----BEGIN RSA PUBLIC KEY-----
MIIBCwKCAQEA9Rju2whruXDVQZpfylVEPeNxm7XgMHcDyaaRUIpXQE0qEo+6Y36L
P0xpFvL0H0kKxHwpuISsdgrnMHJ/yj4S61MWzhO8y4BQbw/zJehhDSRCecFJmFBz
3I2JC5FCjoK+82xd9xM5XXdfsdBzRiSghuIHL4qk2WZ/0f/nK5VygeWXn/oLeYBL
jX1S8wSSASza64JXjt0bP/i6mpV2SLZqKRxo7x2bIQrR1yHNekSF2jBhZIgcbtMB
xjCywn+7p954wjcfjxB5VWaZ4hGbKhi1bhYPccht4XnGhcUTWO3NmJWslwccjQ4k
sutLq3uRjLMM0IeTkQO6Pv8/R7UNFtdCWwIERzH8IQ==
-----END RSA PUBLIC KEY-----";

        public const string DATA1 = @"-----BEGIN RSA PUBLIC KEY-----
MIIBCwKCAQEAxaBCHQJrtLJiJNdG9nq3deA9sY4YCZ4dbTOHO+v+YgWRMcE6iK6o
ZIJq+nBMUNBbGPmbRrEjkkH9M7LAypAFOPKC6wMHzqIMBsUMuYffulBuOqtEBD11
CAwfx37rjwJ+/1tnEqtJjYkrK9yyrIN6Y+jy4ftymQtjk83+L89pvMMmkNeZaPON
4O9q5M9PnFoKvK8eY45ZV/Jyk+Pe+xc6+e4h4cx8ML5U2kMM3VDAJush4z/05hS3
/bC4B6K9+7dPwgqZgKx1J7DBtLdHSAgwRPpijPeOjKcAa2BDaNp9Cfon70oC+ZCB
+HkQ7FjJcF7KaHsH5oHvuI7EZAl2XTsLEQIENa/2JQ==
-----END RSA PUBLIC KEY-----";

        public const string DATA2 = @"-----BEGIN RSA PUBLIC KEY-----
MIIBDAKCAQEA0iDVVQ230RgrkIHJNDgxE7I/2AaH6Li1Eu9mtpfrrfhfoK2e7y4O
WU+lj7AGI4GIgkWpPw8JHaV970Cr6+sTG4Tr5eMQPxrCIH7BJAPCloypxcs2BNfT
GXzm6veUfrGzLIDp7wy24lIA8r9ZwUvpKlN28kxBDGeCbGCkYeSVNuF+R9rN4OAM
RYh0r1Q950xc2qSNloNsjpDoSKoYN0T7u5rnMn/4mtclnWPVRWU940zr1rymv4Jc
3umNf6cT1XqrS1gSaK1JWZfsSeD6Dwk3uvquvfY6YlGRygIlVEMAvKrDRMHylsLt
qqhYkZNXMdy0NXopf1rEHKy9poaHEmJldwIFAP////8=
-----END RSA PUBLIC KEY-----";

        public const string DATA3 = @"-----BEGIN RSA PUBLIC KEY-----
MIIBCwKCAQEAvRRNBnVq3WknCNHrJRelcEA2v/OzKlQkxZw1yKll0Y2Kn6G9ts94
SfgZYbdFCnIXy5NEuyHRKrxXz5vurjhrcuoYAI2ZUhXPXZJdgHywac/i3S/IY0V/
eDbqepyJWHpP6I565ySqlol1p/BScVjbEsVyvZGtWIXLPDbx4EYFKA5B52uK6Gdz
4qcyVFtVEhNoMvg+EoWnyLD7EUzuB2Khl46CuNictyWrLlIHgpKJr1QD8a0ld0PD
PHDZn03q6QDvZd23UW2d9J+/HeBt52j08+qoBXPwhndZsmPMWngQDaik6FM7EVRQ
etKPi6h5uprVmMAS5wR/jQIVTMpTj/zJdwIEXszeQw==
-----END RSA PUBLIC KEY-----";

        public const string SD = @"-----BEGIN RSA PUBLIC KEY-----
MIIBCwKCAQEAmYJ/5GJU4boJSvZ81BFOHYTGdBWPHnWYly3yWo01BYjGRnz8NTkz
DHUxsbjIgtG5XqsQfZstZILQ97hgSI5AaAoCGrT8sn0PeXg2i0mKwL21gRjRUdvP
Dp1Y+7hgrGwuTkjycqqsQ/qILm4NvJHvGRd7xLOJ9rs2zwYhceRVrq9XU2AXbdY4
pdCQ3+HuoaFiJ0dW0ly5qdEXjbSv2QEYe36nWCtsd6hEY9LjbBX8D1fK3D2c6C0g
NdHJGH2iEONUN6DMK9t0v2JBnwCOZQ7W+Gt7SpNNrkx8xKEM8gH9na10g9ne11Mi
O1FnLm8i4zOxVdPHQBKICkKcGS1o3C2dfwIEXw/f3w==
-----END RSA PUBLIC KEY-----";
    } // TODO revisit if this is correct for the DLC
}
