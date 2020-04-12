namespace FsGame.Resources

open Foom

type MeshResource =
    | WadMap of Wad.Wad * mapName: string
