﻿
using Assets.Data.CASC;
using Assets.WoWEditSettings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Data.WoW_Format_Parsers.ADT
{
    public static partial class ADT
    {
        // Thread Status //
        public static bool ThreadWorkingMesh;
        public static bool ThreadWorkingTextures;
        public static bool ThreadWorkingModels;
        public static float finishedTimeTerrainMesh;
        public static float finishedTimeTerrainTextures;
        public static float finishedTimeTerrainModels;

        // Precalculated Mesh Data //
        public static Vector3[] Chunk_Vertices;
        public static Vector3[] Chunk_VerticesLoD1;
        public static int[] Chunk_Triangles;
        public static int[] Chunk_TrianglesLoD1;
        public static Vector2[] Chunk_UVs;
        public static List<Vector2[]> Chunk_UVs2;
        public static Vector2[] Chunk_UVsLod1;

        public static bool working = false;

        // Run at Startup to Precalculate Some of the Chunk Mesh Data //
        public static void Initialize()
        {
            ////////////////////////////////////////
            #region Verts LoD0 (needs fix, unused)

            Chunk_Vertices = new Vector3[145];
            int currentVertex = 0;
            for (int i = 0; i < 17; i++)
            {
                if (i % 2 == 0)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        Chunk_Vertices[currentVertex] = new Vector3((float)((-i * 0.5) * 0.208333125 * 20) / Settings.WorldSettings.WorldScale,
                                                                            0,
                                                                            (float)(-j * 0.208333125 * 20) / Settings.WorldSettings.WorldScale);
                        currentVertex++;
                    }
                }
                if (i % 2 == 1)
                {
                    for (int j1 = 0; j1 < 8; j1++)
                    {
                        Chunk_Vertices[currentVertex] = new Vector3((float)(-i * 0.5 * 0.208333125 * 20) / Settings.WorldSettings.WorldScale,
                                                                            0,
                                                                            (float)((-j1 - 0.5) * 0.208333125 * 20) / Settings.WorldSettings.WorldScale);
                        currentVertex++;
                    }
                }
            }

            #endregion
            ////////////////////////////////////////

            ////////////////////////////////////////
            #region Verts LoD1 (unused)
            /*
            Chunk_VerticesLoD1 = new Vector3[81];
            int currentVert = 0;
            int currentWriteVert = 0;
            for (int v = 0; v < 17; v++)
            {
                if (v % 2 == 0)
                {
                    for (int v1 = 0; v1 < 9; v1++)
                    {
                        Chunk_VerticesLoD1[currentWriteVert] = Chunk_Vertices[currentVert];
                        currentVert++;
                        currentWriteVert++;
                    }
                }
                else
                {
                    currentVert = currentVert + 8;
                }
            }
            */
            #endregion
            ////////////////////////////////////////

            ////////////////////////////////////////
            #region Triangles LoD0

            Chunk_Triangles = new int[256 * 3];
            int triOffset = 0;
            //create 8 strips//
            for (int strip = 0; strip < 8; strip++)
            {
                //   case \/   //
                for (int t = 0; t < 8; t++)
                {
                    Chunk_Triangles[triOffset + 0] = t + strip * 17;
                    Chunk_Triangles[triOffset + 1] = t + 1 + strip * 17;
                    Chunk_Triangles[triOffset + 2] = t + 9 + strip * 17;
                    triOffset = triOffset + 3;
                }
                //   case /\   //
                for (int t1 = 0; t1 < 8; t1++)
                {
                    Chunk_Triangles[triOffset + 0] = t1 + (strip + 1) * 17;
                    Chunk_Triangles[triOffset + 1] = t1 - 8 + (strip + 1) * 17;
                    Chunk_Triangles[triOffset + 2] = t1 + 1 + (strip + 1) * 17;
                    triOffset = triOffset + 3;
                }
                //   case >   //
                for (int t2 = 0; t2 < 8; t2++)
                {
                    Chunk_Triangles[triOffset + 0] = t2 + strip * 17;
                    Chunk_Triangles[triOffset + 1] = t2 + 9 + strip * 17;
                    Chunk_Triangles[triOffset + 2] = t2 + 17 + strip * 17;
                    triOffset = triOffset + 3;
                }
                //   case <   //
                for (int t3 = 0; t3 < 8; t3++)
                {
                    Chunk_Triangles[triOffset + 0] = t3 + 9 + strip * 17;
                    Chunk_Triangles[triOffset + 1] = t3 + 1 + strip * 17;
                    Chunk_Triangles[triOffset + 2] = t3 + 18 + strip * 17;
                    triOffset = triOffset + 3;
                }
            }
            #endregion
            ////////////////////////////////////////

            ////////////////////////////////////////
            #region Triangles LoD1 (unused)
            /*
            Chunk_TrianglesLoD1 = new int[128 * 3];
            int triOffset1 = 0;
            //create 8 strips//
            for (int strip = 0; strip < 8; strip++)
            {
                //   case ⅂   //
                for (int t = 0; t < 8; t++)
                {
                    Chunk_TrianglesLoD1[triOffset1 + 0] = t + strip * 9;
                    Chunk_TrianglesLoD1[triOffset1 + 1] = t + 1 + strip * 9;
                    Chunk_TrianglesLoD1[triOffset1 + 2] = t + 10 + strip * 9;
                    triOffset1 = triOffset1 + 3;
                }
                //   case L   //
                for (int t1 = 0; t1 < 8; t1++)
                {
                    Chunk_TrianglesLoD1[triOffset1 + 0] = t1 + strip * 9;
                    Chunk_TrianglesLoD1[triOffset1 + 1] = t1 + 10 + strip * 9;
                    Chunk_TrianglesLoD1[triOffset1 + 2] = t1 + 9 + strip * 9;
                    triOffset1 = triOffset1 + 3;
                }
            }
            */
            #endregion
            ////////////////////////////////////////

            ////////////////////////////////////////
            #region UV's LoD0

            Chunk_UVs = new Vector2[145];
            for (int u = 144; u >= 0; u--)
            {
                Chunk_UVs[u] = new Vector2(-(Chunk_Vertices[u].z / (33.3333f / Settings.WorldSettings.WorldScale)),
                                           -(Chunk_Vertices[u].x / (33.3333f / Settings.WorldSettings.WorldScale)));
            }

            #endregion
            ////////////////////////////////////////

            ////////////////////////////////////////
            #region UV's2 LoD0

            Chunk_UVs2 = new List<Vector2[]>();
            for (int c = 16; c > 0; c--)
            {
                for (int r = 16; r > 0; r--)
                {
                    Vector2[] UVs = new Vector2[145];
                    for (int u = 0; u < 145; u++)
                    {
                        UVs[u] = new Vector2(1 - ((Chunk_Vertices[u].z / (33.3333f / Settings.WorldSettings.WorldScale) * 0.0625f) + (r * 0.0625f)),
                                             1 - ((Chunk_Vertices[u].x / (33.3333f / Settings.WorldSettings.WorldScale) * 0.0625f) + (c * 0.0625f)));
                    }
                    Chunk_UVs2.Add(UVs);
                }
            }


            #endregion
            ////////////////////////////////////////

            ////////////////////////////////////////
            #region UV's LoD1 (unused)
            /*
            Chunk_UVsLod1 = new Vector2[81];
            for (int u = 0; u < 81; u++)
            {
                Chunk_UVsLod1[u] = new Vector2(Chunk_VerticesLoD1[u].x / (33.3333f / Settings.worldScale),
                                               Chunk_VerticesLoD1[u].z / (33.3333f / Settings.worldScale));
            }
            */
            #endregion
            ////////////////////////////////////////
        }

        // Run Terrain Mesh Parser //
        public static void LoadTerrainMesh(string Path, string MapName, Vector2 Coords)
        {
            ThreadWorkingMesh = true;
            long millisecondsStart = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            ADTRootData.meshBlockData = new ADTRootData.MeshBlockData();
            ADTRootData.meshBlockData.meshChunksData = new List<ADTRootData.MeshChunkData>();

            ParseADT_Main(Path, MapName, Coords);
            ADT_ProcessData.GenerateMeshArrays();

            if (working)
                ADTRootData.MeshBlockDataQueue.Enqueue(ADTRootData.meshBlockData);

            long millisecondsStop = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            finishedTimeTerrainMesh = (millisecondsStop - millisecondsStart) / 1000f;

            ThreadWorkingMesh = false;
        }

        // Run Terrain Texture Parser //
        public static void LoadTerrainTextures(string Path, string MapName, Vector2 Coords)
        {
            ThreadWorkingTextures   = true;
            long millisecondsStart  = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            ADTTexData.textureBlockData                     = new ADTTexData.TextureBlockData();
            ADTTexData.textureBlockData.textureChunksData   = new List<ADTTexData.TextureChunkData>();

            ParseADT_Tex(Path, MapName, Coords);
            if (SettingsManager<Configuration>.Config.TerrainImport.LoadShadowMaps)
                ADT_ProcessData.AdjustAlphaBasedOnShadowmap(MapName);
            ADT_ProcessData.Load_hTextures();

            if (working)
                ADTTexData.TextureBlockDataQueue.Enqueue(ADTTexData.textureBlockData);

            long millisecondsStop       = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            finishedTimeTerrainTextures = (millisecondsStop - millisecondsStart) / 1000f;

            ThreadWorkingTextures       = false;
        }

        // Run Terrain Models Parser //
        public static void LoadTerrainModels(string Path, string MapName, Vector2 Coords)
        {
            ThreadWorkingModels = true;
            long millisecondsStart = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            ADTObjData.modelBlockData = new ADTObjData.ModelBlockData();
            ADTObjData.modelBlockData.terrainPos = Coords;
            if (Settings.terrainImport.LoadWMOs ||
                Settings.terrainImport.LoadM2s)
                ParseADT_Obj(Path, MapName, Coords);

            if (working)
                ADTObjData.ModelBlockDataQueue.Enqueue(ADTObjData.modelBlockData);

            long millisecondsStop = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            finishedTimeTerrainModels = (millisecondsStop - millisecondsStart) / 1000f;

            ThreadWorkingModels = false;
        }

        // Terrain Mesh Parser //
        private static void ParseADT_Main(string Path, string MapName, Vector2 coords)  // MS version
        {
            ADTRoot r = new ADTRoot();
            string ADTmainPath = Path + MapName + "_" + coords.x + "_" + coords.y + ".adt";
            int filedata    = Casc.GetFileDataIdByName(ADTmainPath);
            string path     = Casc.GetFile(filedata);
            byte[] ADTmainData = File.ReadAllBytes(path);

            int MCNKchunkNumber = 0;
            // long streamPosition = 0;

            using (MemoryStream ms = new MemoryStream(ADTmainData))
            using (BinaryReader s = new BinaryReader(ms))
            {
                while (ms.Position < ms.Length)
                {
                    //ms.Position = streamPosition;
                    ADTChunkId chunkID = (ADTChunkId)s.ReadInt32();
                    int chunkSize = s.ReadInt32();
                    //streamPosition = ms.Position + chunkSize;

                    switch (chunkID)
                    {
                        case ADTChunkId.MVER:
                            r.ReadMVER(s); // ADT file version
                            break;
                        case ADTChunkId.MHDR:
                            r.ReadMHDR(s); // Offsets for specific chunks 0000 if chunks don't exist.
                            break;
                        case ADTChunkId.MH2O:
                            r.ReadMH2O(s, chunkSize); // Water Data
                            break;
                        case ADTChunkId.MCNK:
                            {
                                r.ReadMCNK(s, MCNKchunkNumber, chunkSize); // Terrain Data - 256chunks
                                MCNKchunkNumber++;
                            }
                            break;
                        case ADTChunkId.MFBO:
                            r.ReadMFBO(s); // FlightBounds plane & Death plane
                            break;
                        default:
                            ADTRoot.SkipUnknownChunk(s, chunkID, chunkSize);
                            break;
                    }
                }
            }
            ADTmainData = null;
        }

        // Terrain Texture Parser //
        private static void ParseADT_Tex(string Path, string MapName, Vector2 coords)
        {
            ADTTex t = new ADTTex();
            string ADTtexPath = Path + MapName + "_" + coords.x + "_" + coords.y + "_tex0" + ".adt";
            int filedata = Casc.GetFileDataIdByName(ADTtexPath);
            string path = Casc.GetFile(filedata);

            byte[] ADTtexData = File.ReadAllBytes(path);

            int MCNKchunkNumber = 0;
            long streamPosition = 0;

            using (MemoryStream ms = new MemoryStream(ADTtexData))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                while (streamPosition < ms.Length)
                {
                    ms.Position = streamPosition;
                    ADTChunkId chunkID = (ADTChunkId)reader.ReadInt32();
                    int chunkSize = reader.ReadInt32();
                    streamPosition = ms.Position + chunkSize;

                    switch (chunkID)
                    {
                        case ADTChunkId.MVER:
                            t.ReadMVER(reader); // ADT file version
                            break;
                        case ADTChunkId.MAMP:
                            t.ReadMAMP(reader); // Single value - texture size = 64
                            break;
                        case ADTChunkId.MTEX:
                            t.ReadMTEX(reader, chunkSize); // Texture Paths
                            break;
                        case ADTChunkId.MCNK:
                            {
                                t.ReadMCNKtex(reader, MapName, MCNKchunkNumber, chunkSize); // Texture Data - 256chunks
                                MCNKchunkNumber++;
                            }
                            break;
                        case ADTChunkId.MTXP:
                            t.ReadMTXP(reader, chunkSize);
                            break;
                        case ADTChunkId.MHID:
                            t.ReadMHID(reader, chunkSize);
                            break;
                        case ADTChunkId.MDID:
                            t.ReadMDID(reader, chunkSize);
                            break;
                        default:
                            ADTTex.SkipUnknownChunk(reader, chunkID, chunkSize);
                            break;
                    }
                }
            }
            ADTtexData = null;
        }

        // Terrain Models Parser //
        public static void ParseADT_Obj(string Path, string MapName, Vector2 coords)
        {
            ADTObj o = new ADTObj();
            string ADTobjPath = Path + MapName + "_" + coords.x + "_" + coords.y + "_obj0" + ".adt";
            int filedata = Casc.GetFileDataIdByName(ADTobjPath);
            string path = Casc.GetFile(filedata);

            byte[] ADTobjData = File.ReadAllBytes(path);

            int MCNKchunkNumber = 0;
            long streamPosition = 0;
            using (MemoryStream ms = new MemoryStream(ADTobjData))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                while (streamPosition < ms.Length)
                {
                    ms.Position = streamPosition;
                    ADTChunkId chunkID = (ADTChunkId)reader.ReadInt32();
                    int chunkSize = reader.ReadInt32();
                    streamPosition = ms.Position + chunkSize;

                    switch (chunkID)
                    {
                        case ADTChunkId.MVER:
                            o.ReadMVER(reader); // ADT file version
                            break;
                        case ADTChunkId.MDDF:
                            o.ReadMDDF(reader, chunkSize); // Placement information for doodads (M2 models).
                            break;
                        case ADTChunkId.MODF:
                            o.ReadMODF(reader, chunkSize); // Placement information for WMOs.
                            break;
                        case ADTChunkId.MCNK:
                            {
                                o.ReadMCNKObj(reader, MapName, MCNKchunkNumber, chunkSize); // 256chunks
                                MCNKchunkNumber++;
                            }
                            break;
                        default:
                            SkipUnknownChunk(reader, chunkID, chunkSize);
                            break;
                    }
                }
            }
        }

        // Move the stream forward upon finding unknown chunks //
        public static void SkipUnknownChunk(BinaryReader reader, ADTChunkId chunkID, int chunkSize)
        {
            Debug.Log("Missing chunk ID : " + chunkID);
            reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
        }

    }
}