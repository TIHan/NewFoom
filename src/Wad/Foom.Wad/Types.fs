namespace Foom.Wad

open System
open System.Collections.Immutable
open Foom.Geometry

type Sidedef = 
    {
        OffsetX: int
        OffsetY: int
        UpperTextureName: string option
        LowerTextureName: string option
        MiddleTextureName: string option
        SectorNumber: int 
    }

[<Flags>]
type LinedefFlags =
    | Empty = 0x0000
    | BlocksPlayersAndMonsters = 0x0001
    | BlocksMonsters = 0x0002
    | TwoSided = 0x0004
    | UpperTextureUnpegged = 0x0008
    | LowerTextureUnpegged = 0x0010
    | Secret = 0x0020
    | BlocksSound = 0x0040
    | NerverShowsOnAutomap = 0x0080
    | AlwaysShowsOnAutomap = 0x0100

type Linedef = 
    {
        Segment: LineSegment2D
        FrontSidedefIndex: int option
        BackSidedefIndex: int option
        Flags: LinedefFlags
        SpecialType: int
        SectorTag: int
    }

type SectorType =
    | Normal = 0
    | BlinkLightRandom = 1
    | BlinkLightHalfASecond = 2
    | BlinkLightdOneSecond = 3
    | TwentyPercentDamagePerSecondPlusBlinkLightHalfASecond = 4
    | TenPercentDamagePerSecond = 5
    | FivePercentDamagePerSecond = 7
    | LightOscillates = 8
    | PlayerEnteringSectorGetsCreditForFindingASecret = 9
    | ThirtySecondsAfterLevelStartCeilingClosesLikeADoor = 10
    | CancelGodModeAndTwentyPercentDamagePerSecondAndWhenPlayerDiesLevelEnds = 11
    | BlinkLightHalfASecondSync = 12
    | BlinkLightOneSecondSync = 13
    | ThreeHundredSecondsAfterLevelStartCeilingOpensLikeADoor = 14
    | TwentyPercentDamagePerSecond = 16
    | FlickerLightRandomly = 17

type Sector = 
    {
        Id: int
        FloorTextureName: string
        FloorHeight: int
        CeilingTextureName: string
        CeilingHeight: int
        LightLevel: int
        Type: SectorType
        Tag: int
    } 

type ThingType =
    // Artifact Items
    | Berserk = 2023
    | ComputerMap = 2026
    | HealthBonus = 2014
    | Invisibility = 2024
    | Invulnerability = 2022
    | LightAmplificationVisor = 2045
    | Megasphere = 83
    | SoulSphere = 2013
    | ArmorBonus = 2015

    // Powerups
    | Backpack = 8
    | BlueArmor = 2019
    | GreenArmor = 2018
    | Medkit = 2012
    | RadiationSuit = 2025
    | Stimpack = 2011

    // Weapons
    | BFG9000 = 2006
    | Chaingun = 2002
    | Chainsaw = 2005
    | PlasmaGun = 2004
    | RocketLauncher = 2003
    | Shotgun = 2001
    | SuperShotgun = 82

    // Ammunition
    | AmmoClip = 2007
    | BoxOfAmmo = 2048
    | BoxOfRockets = 2046
    | BoxOfShells = 2049
    | EnergyCell = 2047
    | EnergyCellPack = 17
    | Rocket = 2010
    | ShotgunShells = 2008

    // Keys
    | BlueKeycard = 5
    | BlueSkullKey = 40
    | RedKeycard = 13
    | RedSkullKey = 38
    | YellowKeycard = 6
    | YellowSkullKey = 39

    // Monsters
    | Arachnotron = 68
    | ArchVile = 64
    | BaronOfHell = 3003
    | Cacodemon = 3005
    | HeavyWeaponDude = 65
    | CommanderKeen = 72
    | Cyberdemon = 16
    | Demon = 3002
    | Zombieman = 3004
    | ShotgunGuy = 9
    | HellKnight = 69
    | Imp = 3001
    | LostSoul = 3006
    | Mancubus = 67
    | PainElemental = 71
    | Revenant = 66
    | Spectre = 58
    | Spiderdemon = 7
    | WolfensteinSS = 84

    // Obstacles
    | Barrel = 2035
    | BurningBarrel = 70
    | BurntTree = 43
    | Candelabra = 35
    | EvilEye = 41
    | FiveSkullsKebab = 28
    | FloatingSkull = 42
    | FloorLamp = 2028
    | HangingLeg = 53
    | HangingPairOfLegs = 52
    | HangingTorsoBrainRemoved = 78
    | HangingTorsoLookingDown = 75
    | HangingTorsoLookupUp = 77
    | HangingTorsoOpenSkull = 76
    | HangingVictimArmsOut = 50
    | HangingVictimGutsAndBrainRemoved = 74
    | HangingVictimGutsRemoved = 73
    | HangingVictimOneLegged = 51
    | HangingVictimTwitching = 49
    | ImpaledHuman = 25
    | LargeBrownTree = 54
    | PileOfSkullsAndCandles = 29
    | ShortBlueFirestick = 55
    | ShotGreenFirestick = 56
    | ShortGreenPillar = 31
    | ShortGreenPillarWithBeatingHeart = 36
    | ShortRedFirestick = 57
    | ShortRedPillar = 33
    | ShortRedPIllarWithSkull = 37
    | ShortTechnoFloorLamp = 86
    | SkullOnAPole = 27
    | Stalagmite = 47
    | TallBlueFirestick = 44
    | TallGreenFirestick = 45
    | TallGreenPillar = 30
    | TallRedFirestick = 46
    | TallRedPillar = 32
    | TallTechnoFloorLamp = 85
    | TallTechnoPillar = 48
    | TwitchingImpaledHuamn = 26

    // Decorations
    | BloodyMess = 10
    | BloodyMess2 = 12
    | Candle = 34
    | DeadCacodemon = 22
    | DeadDemon = 21
    | DeadFormerHuman = 18
    | DeadFormerSergeant = 19
    | DeadImp = 20
    | DeadLostSoulInvisible = 23
    | DeadPlayer = 15
    | HangingLegDecoration = 62
    | HangingPairOfLegsDecoration = 60
    | HangingVictimArmsOutDecoration = 59
    | HangingVictimOneLeggedDecoration = 61
    | HangingVictimTwitchingDecoration = 63
    | PoolOfBlood = 79
    | PoolOfBlood2 = 80
    | PoolOfBloodAndFlesh = 24
    | PoolOfBrains = 81

    // Others
    | BossBrain = 88
    | DeatmatchStart = 11
    | Player1Start = 1
    | Player2Start = 2
    | Player3Start = 3
    | Player4Start = 4
    | SpawnShooter = 89
    | SpawnSpot = 87
    | TeleportLanding = 14

[<Flags>]
type DoomThingFlags =
    | SkillLevelOneAndTwo = 0x001
    | SkillLevelThree = 0x002
    | SkillLevelFourAndFive = 0x004
    | Deaf = 0x008
    | NotInSinglePlayer = 0x0010
    | NotInDeathmatch = 0x0020 // boom
    | NotInCoop = 0x0040 // boom
    | FriendlyMonster = 0x0080 // MBF

[<Flags>]
type HexenThingFlags =
    | SkillLevelOneAndTwo = 0x001
    | SkillLevelThree = 0x002
    | SkillLevelFourAndFive = 0x004
    | Deaf = 0x008
    | Dormant = 0x0010
    | AppearOnlyToFighterClass = 0x0020
    | AppearOnlyToClericClass = 0x0040
    | AppearOnlyToMageClass = 0x0080
    | AppearOnlyInSinglePlayer = 0x0100
    | AppearOnlyInCoop = 0x0200
    | AppearOnlyInDeathmatch = 0x0400
 
type DoomThing = { X: int; Y: int; Angle: int; Type: ThingType; Flags: DoomThingFlags }

type HexenThing = { Id: int; X: int; Y: int; StartingHeight: int; Angle: int; Flags: HexenThingFlags; Arg1: byte; Arg2: byte; Arg3: byte; Arg4: byte; Arg5: byte }

[<RequireQualifiedAccess>]
type Thing =
    | Doom of DoomThing
    | Hexen of HexenThing

open System.Collections.Generic

type Map =
    {
        Sectors: ImmutableArray<Sector>
        Things: ImmutableArray<Thing>
        Linedefs: ImmutableArray<Linedef>
        Sidedefs: ImmutableArray<Sidedef>
    }

    static member Create (sectors: Sector seq, things: Thing seq, linedefs: Linedef seq, sidedefs: Sidedef seq) =
        {
            Sectors = sectors.ToImmutableArray()
            Things = things.ToImmutableArray()
            Linedefs = linedefs.ToImmutableArray()
            Sidedefs = sidedefs.ToImmutableArray()
        }

    member this.TryFindPlayer1Start() =
        this.Things
        |> Seq.choose(function
            | Thing.Doom thing -> Some thing
            | _ -> None
        )
        |> Seq.tryFind(fun thing ->
            if thing.Type = ThingType.Player1Start then true
            else false
        )

[<Struct>]
type Pixel =
    val R : byte
    val G : byte
    val B : byte

    new (r, g, b) = { R = r; G = g; B = b }

    static member Cyan = Pixel (0uy, 255uy, 255uy)