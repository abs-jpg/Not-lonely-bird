#!/usr/bin/env python3
"""Generate 3DGenerate.unity scene YAML."""

# We keep the existing stub's Directional Light (520400733/734/735)
# and Prefab instances (520401423 PointableUI, 520401935 [RKInput], 520402340 RKCameraRig)
# and add all the new GameObjects.

FONT_GUID = "3949d331f3833e340a2575a0496ff63d"
FONT_MAT  = "6280683387203250435"
TMP_GUID  = "f4688fdb7df04437aeb418b961361dc5"  # TextMeshProUGUI
IMG_GUID  = "fe87c0e1cc204ed48ad3b37840f39efc"  # Image
BTN_GUID  = "4e29b1a8efbd4b44bb3f3716e73f07ff"  # Button
CG_GUID   = "d5a6d479c90c98742a199870de28b4d2"  # CanvasGroup (Unity built-in)
SLIDER_GUID = "67db619e29ae940e4bc263c3f1b35f4b"  # Slider (Unity built-in)
TMPINPUT_GUID = "2da0c512f12947e489f739571c563571"  # TMP_InputField
DROPDOWN_GUID = "e3265ab4bf004d28a9537516768c1c75"  # TMP_Dropdown

# Script GUIDs
GLOBALUI_GUID = "ad6e8b055daf4ca49b0ce3e0874fa18c"
SCREENSHOTTER_GUID = "1ce8e9af3b08e1247a154bdf842bfdc2"
EXTERNALINPUT_GUID = "f3e1a662f8d65024bb5a1920ca2959c2"
SCENESCHANGE_GUID = "2bc350ca4d061aa4284be68f689cc26b"

# Third-party GUIDs (referenced but scripts must exist in project)
DRAWINGBOARD_GUID = "9570213ce944ed040a1cbe4da81932fa"
BOOK_GUID = "d5db910d1d5395c43915144358d7a121"
AUTOFLIP_GUID = "35bbf9999c1f06348866e3deff9bdbab"
TRIPOSIMPLEUI_GUID = "09f032cc6eccbc44bad744f19f10d60f"
TRIPORUNTIME_GUID = "73b0d52bf358f9245996499886128aae"
TRIPOMODELLOADER_GUID = "7fc354af441a35b4eab3dd86d3f8d513"
GLTFIMPORT_GUID = "b781fe673a5534e91b1e802df4b9362e"
OVRGRABBABLE_GUID = "777565b974c474bfd8039e9bee64aaf5"
OVRGRABBER_GUID = "27ccf9d23c4ad42b5adc97a680f7ca89"
INTERACTABLE_GUID = "32729db2bca2f44a0a54361d6355894e"
COLLIDERSURFACE_GUID = "63d092618e58346e29c8093d7c9e64a9"
GRABINTERACTABLE_GUID = "aeb5cfd19063548bf82e24d3bb55ba85"
CANVASDRAGHANDLE_GUID = "fd7fed06a72cabb4aa51f4b59e063adc"
ORANGEBTN_GUID = "a1893896ec1d9a340a962cc62a68a676"

# Prefab GUIDs
POINTABLEUI_GUID = "3c20833d81e354626b8365b459274912"
RKINPUT_GUID = "fa2ab4a52b98844a5beea51c6d5ab85a"
RKCAMERARIG_GUID = "bc7bf2e56b74d4038af31f75d0b2d024"

# Material GUIDs
BOARD_MAT_GUID = "4684f07fca5c0644081768edcb2cbc47"
WOOD_MAT_GUID = "c36c3ccb653121243ae5908072c531b6"

lines = []
def w(s=""):
    lines.append(s)

def write_header():
    w("%YAML 1.1")
    w("%TAG !u! tag:unity3d.com,2011:")

def write_scene_settings():
    """OcclusionCulling, RenderSettings, LightmapSettings, NavMesh"""
    w("--- !u!29 &1")
    w("OcclusionCullingSettings:")
    w("  m_ObjectHideFlags: 0")
    w("  serializedVersion: 2")
    w("  m_OcclusionBakeSettings:")
    w("    smallestOccluder: 5")
    w("    smallestHole: 0.25")
    w("    backfaceThreshold: 100")
    w("  m_SceneGUID: 00000000000000000000000000000000")
    w("  m_OcclusionCullingData: {fileID: 0}")
    w("--- !u!104 &2")
    w("RenderSettings:")
    w("  m_ObjectHideFlags: 0")
    w("  serializedVersion: 9")
    w("  m_Fog: 0")
    w("  m_FogColor: {r: 0.5, g: 0.5, b: 0.5, a: 1}")
    w("  m_FogMode: 3")
    w("  m_FogDensity: 0.01")
    w("  m_LinearFogStart: 0")
    w("  m_LinearFogEnd: 300")
    w("  m_AmbientSkyColor: {r: 0.212, g: 0.227, b: 0.259, a: 1}")
    w("  m_AmbientEquatorColor: {r: 0.114, g: 0.125, b: 0.133, a: 1}")
    w("  m_AmbientGroundColor: {r: 0.047, g: 0.043, b: 0.035, a: 1}")
    w("  m_AmbientIntensity: 1")
    w("  m_AmbientMode: 0")
    w("  m_SubtractiveShadowColor: {r: 0.42, g: 0.478, b: 0.627, a: 1}")
    w("  m_SkyboxMaterial: {fileID: 10304, guid: 0000000000000000f000000000000000, type: 0}")
    w("  m_HaloStrength: 0.5")
    w("  m_FlareStrength: 1")
    w("  m_FlareFadeSpeed: 3")
    w("  m_HaloTexture: {fileID: 0}")
    w("  m_SpotCookie: {fileID: 10001, guid: 0000000000000000e000000000000000, type: 0}")
    w("  m_DefaultReflectionMode: 0")
    w("  m_DefaultReflectionResolution: 128")
    w("  m_ReflectionBounces: 1")
    w("  m_ReflectionIntensity: 1")
    w("  m_CustomReflection: {fileID: 0}")
    w("  m_Sun: {fileID: 520400734}")
    w("  m_IndirectSpecularColor: {r: 0, g: 0, b: 0, a: 1}")
    w("  m_UseRadianceAmbientProbe: 0")
    # LightmapSettings
    w("--- !u!157 &3")
    w("LightmapSettings:")
    w("  m_ObjectHideFlags: 0")
    w("  serializedVersion: 12")
    w("  m_GIWorkflowMode: 1")
    w("  m_GISettings:")
    w("    serializedVersion: 2")
    w("    m_BounceScale: 1")
    w("    m_IndirectOutputScale: 1")
    w("    m_AlbedoBoost: 1")
    w("    m_EnvironmentLightingMode: 0")
    w("    m_EnableBakedLightmaps: 1")
    w("    m_EnableRealtimeLightmaps: 0")
    w("  m_LightmapEditorSettings:")
    w("    serializedVersion: 12")
    w("    m_Resolution: 2")
    w("    m_BakeResolution: 40")
    w("    m_AtlasSize: 1024")
    w("    m_AO: 0")
    w("    m_AOMaxDistance: 1")
    w("    m_CompAOExponent: 1")
    w("    m_CompAOExponentDirect: 0")
    w("    m_ExtractAmbientOcclusion: 0")
    w("    m_Padding: 2")
    w("    m_LightmapParameters: {fileID: 0}")
    w("    m_LightmapsBakeMode: 1")
    w("    m_TextureCompression: 1")
    w("    m_FinalGather: 0")
    w("    m_FinalGatherFiltering: 1")
    w("    m_FinalGatherRayCount: 256")
    w("    m_ReflectionCompression: 2")
    w("    m_MixedBakeMode: 2")
    w("    m_BakeBackend: 1")
    w("    m_PVRSampling: 1")
    w("    m_PVRDirectSampleCount: 32")
    w("    m_PVRSampleCount: 500")
    w("    m_PVRBounces: 2")
    w("    m_PVREnvironmentSampleCount: 500")
    w("    m_PVREnvironmentReferencePointCount: 2048")
    w("    m_PVRFilteringMode: 2")
    w("    m_PVRDenoiserTypeDirect: 0")
    w("    m_PVRDenoiserTypeIndirect: 0")
    w("    m_PVRDenoiserTypeAO: 0")
    w("    m_PVRFilterTypeDirect: 0")
    w("    m_PVRFilterTypeIndirect: 0")
    w("    m_PVRFilterTypeAO: 0")
    w("    m_PVREnvironmentMIS: 0")
    w("    m_PVRCulling: 1")
    w("    m_PVRFilteringGaussRadiusDirect: 1")
    w("    m_PVRFilteringGaussRadiusIndirect: 5")
    w("    m_PVRFilteringGaussRadiusAO: 2")
    w("    m_PVRFilteringAtrousPositionSigmaDirect: 0.5")
    w("    m_PVRFilteringAtrousPositionSigmaIndirect: 2")
    w("    m_PVRFilteringAtrousPositionSigmaAO: 1")
    w("    m_ExportTrainingData: 0")
    w("    m_TrainingDataDestination: TrainingData")
    w("    m_LightProbeSampleCountMultiplier: 4")
    w("  m_LightingDataAsset: {fileID: 0}")
    w("  m_LightingSettings: {fileID: 0}")
    # NavMesh
    w("--- !u!196 &4")
    w("NavMeshSettings:")
    w("  serializedVersion: 2")
    w("  m_ObjectHideFlags: 0")
    w("  m_BuildSettings:")
    w("    serializedVersion: 3")
    w("    agentTypeID: 0")
    w("    agentRadius: 0.5")
    w("    agentHeight: 2")
    w("    agentSlope: 45")
    w("    agentClimb: 0.4")
    w("    ledgeDropHeight: 0")
    w("    maxJumpAcrossDistance: 0")
    w("    minRegionArea: 2")
    w("    manualCellSize: 0")
    w("    cellSize: 0.16666667")
    w("    manualTileSize: 0")
    w("    tileSize: 256")
    w("    buildHeightMesh: 0")
    w("    maxJobWorkers: 0")
    w("    preserveTilesOutsideBounds: 0")
    w("    debug:")
    w("      m_Flags: 0")
    w("  m_NavMeshData: {fileID: 0}")

def write_directional_light():
    """Directional Light — reuse existing IDs 520400733/734/735"""
    w("--- !u!1 &520400733")
    w("GameObject:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  serializedVersion: 6")
    w("  m_Component:")
    w("  - component: {fileID: 520400735}")
    w("  - component: {fileID: 520400734}")
    w("  m_Layer: 0")
    w("  m_Name: Directional Light")
    w("  m_TagString: Untagged")
    w("  m_Icon: {fileID: 0}")
    w("  m_NavMeshLayer: 0")
    w("  m_StaticEditorFlags: 0")
    w("  m_IsActive: 1")
    w("--- !u!108 &520400734")
    w("Light:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 520400733}")
    w("  m_Enabled: 1")
    w("  serializedVersion: 10")
    w("  m_Type: 1")
    w("  m_Shape: 0")
    w("  m_Color: {r: 1, g: 0.95686275, b: 0.8392157, a: 1}")
    w("  m_Intensity: 1")
    w("  m_Range: 10")
    w("  m_SpotAngle: 30")
    w("  m_InnerSpotAngle: 21.80208")
    w("  m_CookieSize: 10")
    w("  m_Shadows:")
    w("    m_Type: 2")
    w("    m_Resolution: -1")
    w("    m_CustomResolution: -1")
    w("    m_Strength: 1")
    w("    m_Bias: 0.05")
    w("    m_NormalBias: 0.4")
    w("    m_NearPlane: 0.2")
    w("    m_CullingMatrixOverride:")
    w("      e00: 1")
    w("      e01: 0")
    w("      e02: 0")
    w("      e03: 0")
    w("      e10: 0")
    w("      e11: 1")
    w("      e12: 0")
    w("      e13: 0")
    w("      e20: 0")
    w("      e21: 0")
    w("      e22: 1")
    w("      e23: 0")
    w("      e30: 0")
    w("      e31: 0")
    w("      e32: 0")
    w("      e33: 1")
    w("    m_UseCullingMatrixOverride: 0")
    w("  m_Cookie: {fileID: 0}")
    w("  m_DrawHalo: 0")
    w("  m_Flare: {fileID: 0}")
    w("  m_RenderMode: 0")
    w("  m_CullingMask:")
    w("    serializedVersion: 2")
    w("    m_Bits: 4294967295")
    w("  m_RenderingLayerMask: 1")
    w("  m_Lightmapping: 1")
    w("  m_LightShadowCasterMode: 0")
    w("  m_AreaSize: {x: 1, y: 1}")
    w("  m_BounceIntensity: 1")
    w("  m_ColorTemperature: 6570")
    w("  m_UseColorTemperature: 0")
    w("  m_BoundingSphereOverride: {x: 0, y: 0, z: 0, w: 0}")
    w("  m_UseBoundingSphereOverride: 0")
    w("  m_UseViewFrustumForShadowCasterCull: 1")
    w("  m_ShadowRadius: 0")
    w("  m_ShadowAngle: 0")
    w("--- !u!4 &520400735")
    w("Transform:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 520400733}")
    w("  serializedVersion: 2")
    w("  m_LocalRotation: {x: 0.40821788, y: -0.23456968, z: 0.10938163, w: 0.8754261}")
    w("  m_LocalPosition: {x: 0, y: 3, z: 0}")
    w("  m_LocalScale: {x: 1, y: 1, z: 1}")
    w("  m_ConstrainProportionsScale: 0")
    w("  m_Children: []")
    w("  m_Father: {fileID: 0}")
    w("  m_LocalEulerAnglesHint: {x: 50, y: -30, z: 0}")

# ============================================================
# Helper: write a basic GO + Transform (no RectTransform)
# ============================================================
def go_3d(go_id, tf_id, name, layer=0, components=None, pos=(0,0,0), rot=(0,0,0,1),
          scale=(1,1,1), children=None, father=0, active=1, tag="Untagged",
          constrain=0):
    """Write a 3D GameObject + Transform."""
    comps = [f"  - component: {{fileID: {tf_id}}}"]
    if components:
        for c in components:
            comps.append(f"  - component: {{fileID: {c}}}")
    w(f"--- !u!1 &{go_id}")
    w("GameObject:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  serializedVersion: 6")
    w("  m_Component:")
    for c in comps:
        w(c)
    w(f"  m_Layer: {layer}")
    w(f"  m_Name: {name}")
    w(f"  m_TagString: {tag}")
    w("  m_Icon: {fileID: 0}")
    w("  m_NavMeshLayer: 0")
    w("  m_StaticEditorFlags: 0")
    w(f"  m_IsActive: {active}")
    # Transform
    w(f"--- !u!4 &{tf_id}")
    w("Transform:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w(f"  m_GameObject: {{fileID: {go_id}}}")
    w("  serializedVersion: 2")
    w(f"  m_LocalRotation: {{x: {rot[0]}, y: {rot[1]}, z: {rot[2]}, w: {rot[3]}}}")
    w(f"  m_LocalPosition: {{x: {pos[0]}, y: {pos[1]}, z: {pos[2]}}}")
    w(f"  m_LocalScale: {{x: {scale[0]}, y: {scale[1]}, z: {scale[2]}}}")
    w(f"  m_ConstrainProportionsScale: {constrain}")
    if children:
        w("  m_Children:")
        for ch in children:
            w(f"  - {{fileID: {ch}}}")
    else:
        w("  m_Children: []")
    w(f"  m_Father: {{fileID: {father}}}")
    rx, ry, rz = 0, 0, 0
    # Simple euler hint from quaternion — just store 0s unless specified
    w(f"  m_LocalEulerAnglesHint: {{x: {rx}, y: {ry}, z: {rz}}}")

# ============================================================
# Helper: write a UI GO + RectTransform
# ============================================================
def go_ui(go_id, rt_id, name, layer=5, components=None, anchor_min=(0,0),
          anchor_max=(1,1), pivot=(0.5,0.5), pos=(0,0), size=(100,100),
          scale=(1,1,1), children=None, father=0, active=1):
    comps = [f"  - component: {{fileID: {rt_id}}}"]
    if components:
        for c in components:
            comps.append(f"  - component: {{fileID: {c}}}")
    w(f"--- !u!1 &{go_id}")
    w("GameObject:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  serializedVersion: 6")
    w("  m_Component:")
    for c in comps:
        w(c)
    w(f"  m_Layer: {layer}")
    w(f"  m_Name: {name}")
    w("  m_TagString: Untagged")
    w("  m_Icon: {fileID: 0}")
    w("  m_NavMeshLayer: 0")
    w("  m_StaticEditorFlags: 0")
    w(f"  m_IsActive: {active}")
    # RectTransform
    w(f"--- !u!224 &{rt_id}")
    w("RectTransform:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w(f"  m_GameObject: {{fileID: {go_id}}}")
    w(f"  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}")
    w(f"  m_LocalPosition: {{x: 0, y: 0, z: 0}}")
    w(f"  m_LocalScale: {{x: {scale[0]}, y: {scale[1]}, z: {scale[2]}}}")
    w("  m_ConstrainProportionsScale: 0")
    if children:
        w("  m_Children:")
        for ch in children:
            w(f"  - {{fileID: {ch}}}")
    else:
        w("  m_Children: []")
    w(f"  m_Father: {{fileID: {father}}}")
    w(f"  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}")
    w(f"  m_AnchorMin: {{x: {anchor_min[0]}, y: {anchor_min[1]}}}")
    w(f"  m_AnchorMax: {{x: {anchor_max[0]}, y: {anchor_max[1]}}}")
    w(f"  m_AnchoredPosition: {{x: {pos[0]}, y: {pos[1]}}}")
    w(f"  m_SizeDelta: {{x: {size[0]}, y: {size[1]}}}")
    w(f"  m_Pivot: {{x: {pivot[0]}, y: {pivot[1]}}}")

def write_monobehaviour(fid, go_id, guid, extra=""):
    w(f"--- !u!114 &{fid}")
    w("MonoBehaviour:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w(f"  m_GameObject: {{fileID: {go_id}}}")
    w("  m_Enabled: 1")
    w("  m_EditorHideFlags: 0")
    w(f"  m_Script: {{fileID: 11500000, guid: {guid}, type: 3}}")
    w("  m_Name: ")
    w("  m_EditorClassIdentifier: ")
    if extra:
        w(extra)

def write_tmp(fid, go_id, text="", fontsize=36, color="1,1,1,1", align=514):
    w(f"--- !u!114 &{fid}")
    w("MonoBehaviour:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w(f"  m_GameObject: {{fileID: {go_id}}}")
    w("  m_Enabled: 1")
    w("  m_EditorHideFlags: 0")
    w(f"  m_Script: {{fileID: 11500000, guid: {TMP_GUID}, type: 3}}")
    w("  m_Name: ")
    w("  m_EditorClassIdentifier: ")
    w("  m_Material: {fileID: 0}")
    r,g,b,a = color.split(",")
    w(f"  m_Color: {{r: {r}, g: {g}, b: {b}, a: {a}}}")
    w("  m_RaycastTarget: 1")
    w("  m_RaycastPadding: {x: 0, y: 0, z: 0, w: 0}")
    w("  m_Maskable: 1")
    w(f"  m_OnCullStateChanged:")
    w("    m_PersistentCalls:")
    w("      m_Calls: []")
    w(f'  m_text: "{text}"')
    w("  m_isRightToLeft: 0")
    w(f"  m_fontAsset: {{fileID: 11400000, guid: {FONT_GUID}, type: 2}}")
    w("  m_sharedMaterial: {fileID: " + FONT_MAT + ", guid: " + FONT_GUID + ", type: 2}")
    w("  m_fontMaterials: []")
    w("  m_fontColor32:")
    w("    serializedVersion: 2")
    w(f"    rgba: 4294967295")
    w(f"  m_fontColor: {{r: {r}, g: {g}, b: {b}, a: {a}}}")
    w("  m_enableVertexGradient: 0")
    w("  m_colorMode: 3")
    w("  m_fontColorGradient:")
    w("    topLeft: {r: 1, g: 1, b: 1, a: 1}")
    w("    topRight: {r: 1, g: 1, b: 1, a: 1}")
    w("    bottomLeft: {r: 1, g: 1, b: 1, a: 1}")
    w("    bottomRight: {r: 1, g: 1, b: 1, a: 1}")
    w("  m_fontColorGradientPreset: {fileID: 0}")
    w("  m_spriteAsset: {fileID: 0}")
    w("  m_tintAllSprites: 0")
    w("  m_StyleSheet: {fileID: 0}")
    w("  m_TextStyleHashCode: -1183493901")
    w("  m_overrideHtmlColors: 0")
    w(f"  m_faceColor:")
    w("    serializedVersion: 2")
    w("    rgba: 4294967295")
    w(f"  m_fontSize: {fontsize}")
    w("  m_fontSizeBase: 36")
    w("  m_fontWeight: 400")
    w("  m_enableAutoSizing: 0")
    w("  m_fontSizeMin: 18")
    w("  m_fontSizeMax: 72")
    w("  m_fontStyle: 0")
    w(f"  m_HorizontalAlignment: {align}")
    w("  m_VerticalAlignment: 512")
    w("  m_textAlignment: 65535")
    w("  m_characterSpacing: 0")
    w("  m_wordSpacing: 0")
    w("  m_lineSpacing: 0")
    w("  m_lineSpacingMax: 0")
    w("  m_paragraphSpacing: 0")
    w("  m_charWidthMaxAdj: 0")
    w("  m_enableWordWrapping: 1")
    w("  m_wordWrappingRatios: 0.4")
    w("  m_overflowMode: 0")
    w("  m_linkedTextComponent: {fileID: 0}")
    w("  parentLinkedComponent: {fileID: 0}")
    w("  m_enableKerning: 1")
    w("  m_enableExtraPadding: 0")
    w("  checkPaddingRequired: 0")
    w("  m_isRichText: 1")
    w("  m_parseCtrlCharacters: 1")
    w("  m_isOrthographic: 1")
    w("  m_isCullingEnabled: 0")
    w("  m_horizontalMapping: 0")
    w("  m_verticalMapping: 0")
    w("  m_uvLineOffset: 0")
    w("  m_geometrySortingOrder: 0")
    w("  m_IsTextObjectScaleStatic: 0")
    w("  m_VertexBufferAutoSizeReduction: 0")
    w("  m_useMaxVisibleDescender: 1")
    w("  m_pageToDisplay: 1")
    w("  m_margin: {x: 0, y: 0, z: 0, w: 0}")
    w("  m_isUsingLegacyAnimationComponent: 0")
    w("  m_isVolumetricText: 0")
    w("  m_hasFontAssetChanged: 0")
    w("  m_baseMaterial: {fileID: 0}")
    w("  m_maskOffset: {x: 0, y: 0, z: 0, w: 0}")

def write_image(fid, go_id, color="1,1,1,1", raycast=1):
    r,g,b,a = color.split(",")
    w(f"--- !u!114 &{fid}")
    w("MonoBehaviour:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w(f"  m_GameObject: {{fileID: {go_id}}}")
    w("  m_Enabled: 1")
    w("  m_EditorHideFlags: 0")
    w(f"  m_Script: {{fileID: 11500000, guid: {IMG_GUID}, type: 3}}")
    w("  m_Name: ")
    w("  m_EditorClassIdentifier: ")
    w("  m_Material: {fileID: 0}")
    w(f"  m_Color: {{r: {r}, g: {g}, b: {b}, a: {a}}}")
    w(f"  m_RaycastTarget: {raycast}")
    w("  m_RaycastPadding: {x: 0, y: 0, z: 0, w: 0}")
    w("  m_Maskable: 1")
    w("  m_OnCullStateChanged:")
    w("    m_PersistentCalls:")
    w("      m_Calls: []")
    w("  m_Sprite: {fileID: 0}")
    w("  m_Type: 0")
    w("  m_PreserveAspect: 0")
    w("  m_FillCenter: 1")
    w("  m_FillMethod: 4")
    w("  m_FillAmount: 1")
    w("  m_FillClockwise: 1")
    w("  m_FillOrigin: 0")
    w("  m_UseSpriteMesh: 0")
    w("  m_PixelsPerUnitMultiplier: 1")

def write_canvasgroup(fid, go_id, alpha=1, interactable=1, blocksRaycasts=1):
    w(f"--- !u!225 &{fid}")
    w("CanvasGroup:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w(f"  m_GameObject: {{fileID: {go_id}}}")
    w("  m_Enabled: 1")
    w(f"  m_Alpha: {alpha}")
    w(f"  m_Interactable: {interactable}")
    w(f"  m_BlocksRaycasts: {blocksRaycasts}")
    w("  m_IgnoreParentGroups: 0")

def write_button(fid, go_id, target_id=0, method="", color="1,0.584,0,1"):
    r,g,b,a = color.split(",")
    w(f"--- !u!114 &{fid}")
    w("MonoBehaviour:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w(f"  m_GameObject: {{fileID: {go_id}}}")
    w("  m_Enabled: 1")
    w("  m_EditorHideFlags: 0")
    w(f"  m_Script: {{fileID: 11500000, guid: {BTN_GUID}, type: 3}}")
    w("  m_Name: ")
    w("  m_EditorClassIdentifier: ")
    w("  m_Navigation:")
    w("    m_Mode: 3")
    w("    m_WrapAround: 0")
    w("    m_SelectOnUp: {fileID: 0}")
    w("    m_SelectOnDown: {fileID: 0}")
    w("    m_SelectOnLeft: {fileID: 0}")
    w("    m_SelectOnRight: {fileID: 0}")
    w("  m_Transition: 1")
    w("  m_Colors:")
    w("    m_NormalColor: {r: 1, g: 1, b: 1, a: 1}")
    w("    m_HighlightedColor: {r: 0.9607843, g: 0.9607843, b: 0.9607843, a: 1}")
    w("    m_PressedColor: {r: 0.78431374, g: 0.78431374, b: 0.78431374, a: 1}")
    w("    m_SelectedColor: {r: 0.9607843, g: 0.9607843, b: 0.9607843, a: 1}")
    w("    m_DisabledColor: {r: 0.78431374, g: 0.78431374, b: 0.78431374, a: 0.5019608}")
    w("    m_ColorMultiplier: 1")
    w("    m_FadeDuration: 0.1")
    w("  m_SpriteState:")
    w("    m_HighlightedSprite: {fileID: 0}")
    w("    m_PressedSprite: {fileID: 0}")
    w("    m_SelectedSprite: {fileID: 0}")
    w("    m_DisabledSprite: {fileID: 0}")
    w("  m_AnimationTriggers:")
    w("    m_NormalTrigger: Normal")
    w("    m_HighlightedTrigger: Highlighted")
    w("    m_PressedTrigger: Pressed")
    w("    m_SelectedTrigger: Selected")
    w("    m_DisabledTrigger: Disabled")
    w("  m_Interactable: 1")
    w("  m_TargetGraphic: {fileID: 0}")
    w("  m_OnClick:")
    w("    m_PersistentCalls:")
    if target_id and method:
        w("      m_Calls:")
        w("      - m_Target: {fileID: " + str(target_id) + "}")
        w("        m_TargetAssemblyTypeName: ")
        w("        m_MethodName: " + method)
        w("        m_Mode: 1")
        w("        m_Arguments:")
        w("          m_ObjectArgument: {fileID: 0}")
        w("          m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine")
        w("          m_IntArgument: 0")
        w("          m_FloatArgument: 0")
        w("          m_StringArgument: ")
        w("          m_BoolArgument: 0")
        w("        m_CallState: 2")
    else:
        w("      m_Calls: []")

# ============================================================
# Board (画板) — Layer 6
# fileIDs: GO=100000001, TF=100000002, MeshFilter=100000003,
#          MeshRenderer=100000004, MeshCollider=100000005, DrawingBoard=100000006
# Children: Cube(110), 木脚(120), 木枕(130)
# ============================================================
def write_board():
    go_3d(100000001, 100000002, "Board", layer=6,
           components=[100000003, 100000004, 100000005, 100000006],
           pos=(0.0052, -0.1091, 0.55), scale=(0.5104, 0.4685, 0.05),
           children=[110000002, 120000002, 130000002])
    # MeshFilter — Plane
    w("--- !u!33 &100000003")
    w("MeshFilter:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 100000001}")
    w("  m_Mesh: {fileID: 10210, guid: 0000000000000000e000000000000000, type: 0}")
    # MeshRenderer
    w("--- !u!23 &100000004")
    w("MeshRenderer:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 100000001}")
    w("  m_Enabled: 1")
    w("  m_CastShadows: 1")
    w("  m_ReceiveShadows: 1")
    w("  m_DynamicOccludee: 1")
    w("  m_StaticShadowCaster: 0")
    w("  m_MotionVectors: 1")
    w("  m_LightProbeUsage: 1")
    w("  m_ReflectionProbeUsage: 1")
    w("  m_RayTracingMode: 2")
    w("  m_RayTraceProcedural: 0")
    w("  m_RenderingLayerMask: 1")
    w("  m_RendererPriority: 0")
    w("  m_Materials:")
    w(f"  - {{fileID: 2100000, guid: {BOARD_MAT_GUID}, type: 2}}")
    w("  m_StaticBatchInfo:")
    w("    firstSubMesh: 0")
    w("    subMeshCount: 0")
    w("  m_StaticBatchRoot: {fileID: 0}")
    w("  m_ProbeAnchor: {fileID: 0}")
    w("  m_LightProbeVolumeOverride: {fileID: 0}")
    w("  m_ScaleInLightmap: 1")
    w("  m_ReceiveGI: 1")
    w("  m_PreserveUVs: 0")
    w("  m_IgnoreNormalsForChartDetection: 0")
    w("  m_ImportantGI: 0")
    w("  m_StitchLightmapSeams: 1")
    w("  m_SelectedEditorRenderState: 3")
    w("  m_MinimumChartSize: 4")
    w("  m_AutoUVMaxDistance: 0.5")
    w("  m_AutoUVMaxAngle: 89")
    w("  m_LightmapParameters: {fileID: 0}")
    w("  m_SortingLayerID: 0")
    w("  m_SortingLayer: 0")
    w("  m_SortingOrder: 0")
    w("  m_AdditionalVertexStreams: {fileID: 0}")
    # MeshCollider
    w("--- !u!64 &100000005")
    w("MeshCollider:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 100000001}")
    w("  m_Material: {fileID: 0}")
    w("  m_IncludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_ExcludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_LayerOverridePriority: 0")
    w("  m_IsTrigger: 0")
    w("  m_ProvidesContacts: 0")
    w("  m_Enabled: 1")
    w("  serializedVersion: 4")
    w("  m_Convex: 0")
    w("  m_CookingOptions: 30")
    w("  m_Mesh: {fileID: 10210, guid: 0000000000000000e000000000000000, type: 0}")
    # DrawingBoard script
    write_monobehaviour(100000006, 100000001, DRAWINGBOARD_GUID,
        f"  brushTip: {{fileID: 110000002}}\n"
        f"  brushRaycastDistance: 0.075\n"
        f"  textureWidth: 1024\n"
        f"  textureHeight: 1024\n"
        f"  brushSize: 2\n"
        f"  brushColor: {{r: 0, g: 0, b: 0, a: 1}}\n"
        f"  minPointDistance: 0.1\n"
        f"  lineSubdivisions: 15")
    # Cube (画笔笔尖)
    go_3d(110000001, 110000002, "Cube", layer=6,
           components=[110000003, 110000004],
           pos=(0,0,0), scale=(0.008, 0.003, 0.269),
           father=100000002)
    # Cube MeshFilter
    w("--- !u!33 &110000003")
    w("MeshFilter:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 110000001}")
    w("  m_Mesh: {fileID: 10202, guid: 0000000000000000e000000000000000, type: 0}")
    # Cube MeshRenderer
    w("--- !u!23 &110000004")
    w("MeshRenderer:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 110000001}")
    w("  m_Enabled: 1")
    w("  m_CastShadows: 1")
    w("  m_ReceiveShadows: 1")
    w("  m_DynamicOccludee: 1")
    w("  m_StaticShadowCaster: 0")
    w("  m_MotionVectors: 1")
    w("  m_LightProbeUsage: 1")
    w("  m_ReflectionProbeUsage: 1")
    w("  m_RayTracingMode: 2")
    w("  m_RayTraceProcedural: 0")
    w("  m_RenderingLayerMask: 1")
    w("  m_RendererPriority: 0")
    w("  m_Materials:")
    w("  - {fileID: 10303, guid: 0000000000000000f000000000000000, type: 0}")
    w("  m_StaticBatchInfo:")
    w("    firstSubMesh: 0")
    w("    subMeshCount: 0")
    w("  m_StaticBatchRoot: {fileID: 0}")
    w("  m_ProbeAnchor: {fileID: 0}")
    w("  m_LightProbeVolumeOverride: {fileID: 0}")
    w("  m_ScaleInLightmap: 1")
    w("  m_ReceiveGI: 1")
    w("  m_PreserveUVs: 0")
    w("  m_IgnoreNormalsForChartDetection: 0")
    w("  m_ImportantGI: 0")
    w("  m_StitchLightmapSeams: 1")
    w("  m_SelectedEditorRenderState: 3")
    w("  m_MinimumChartSize: 4")
    w("  m_AutoUVMaxDistance: 0.5")
    w("  m_AutoUVMaxAngle: 89")
    w("  m_LightmapParameters: {fileID: 0}")
    w("  m_SortingLayerID: 0")
    w("  m_SortingLayer: 0")
    w("  m_SortingOrder: 0")
    w("  m_AdditionalVertexStreams: {fileID: 0}")
    # 木脚
    go_3d(120000001, 120000002, "\\u6728\\u811A", layer=6,
           components=[120000003, 120000004, 120000005],
           pos=(0, -0.516, 0.7), scale=(1.1053, 0.088, 2.852),
           father=100000002)
    w("--- !u!33 &120000003")
    w("MeshFilter:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 120000001}")
    w("  m_Mesh: {fileID: 10202, guid: 0000000000000000e000000000000000, type: 0}")
    w("--- !u!23 &120000004")
    w("MeshRenderer:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 120000001}")
    w("  m_Enabled: 1")
    w("  m_CastShadows: 1")
    w("  m_ReceiveShadows: 1")
    w("  m_DynamicOccludee: 1")
    w("  m_StaticShadowCaster: 0")
    w("  m_MotionVectors: 1")
    w("  m_LightProbeUsage: 1")
    w("  m_ReflectionProbeUsage: 1")
    w("  m_RayTracingMode: 2")
    w("  m_RayTraceProcedural: 0")
    w("  m_RenderingLayerMask: 1")
    w("  m_RendererPriority: 0")
    w("  m_Materials:")
    w(f"  - {{fileID: 2100000, guid: {WOOD_MAT_GUID}, type: 2}}")
    w("  m_StaticBatchInfo:")
    w("    firstSubMesh: 0")
    w("    subMeshCount: 0")
    w("  m_StaticBatchRoot: {fileID: 0}")
    w("  m_ProbeAnchor: {fileID: 0}")
    w("  m_LightProbeVolumeOverride: {fileID: 0}")
    w("  m_ScaleInLightmap: 1")
    w("  m_ReceiveGI: 1")
    w("  m_PreserveUVs: 0")
    w("  m_IgnoreNormalsForChartDetection: 0")
    w("  m_ImportantGI: 0")
    w("  m_StitchLightmapSeams: 1")
    w("  m_SelectedEditorRenderState: 3")
    w("  m_MinimumChartSize: 4")
    w("  m_AutoUVMaxDistance: 0.5")
    w("  m_AutoUVMaxAngle: 89")
    w("  m_LightmapParameters: {fileID: 0}")
    w("  m_SortingLayerID: 0")
    w("  m_SortingLayer: 0")
    w("  m_SortingOrder: 0")
    w("  m_AdditionalVertexStreams: {fileID: 0}")
    w("--- !u!65 &120000005")
    w("BoxCollider:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 120000001}")
    w("  m_Material: {fileID: 0}")
    w("  m_IncludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_ExcludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_LayerOverridePriority: 0")
    w("  m_IsTrigger: 0")
    w("  m_ProvidesContacts: 0")
    w("  m_Enabled: 1")
    w("  serializedVersion: 3")
    w("  m_Size: {x: 1, y: 1, z: 1}")
    w("  m_Center: {x: 0, y: 0, z: 0}")
    # 木枕
    go_3d(130000001, 130000002, "\\u6728\\u6795", layer=6,
           components=[130000003, 130000004, 130000005],
           pos=(0, -0.3, 0.5), scale=(1.1053, 0.088, 0.5),
           father=100000002)
    w("--- !u!33 &130000003")
    w("MeshFilter:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 130000001}")
    w("  m_Mesh: {fileID: 10202, guid: 0000000000000000e000000000000000, type: 0}")
    w("--- !u!23 &130000004")
    w("MeshRenderer:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 130000001}")
    w("  m_Enabled: 1")
    w("  m_CastShadows: 1")
    w("  m_ReceiveShadows: 1")
    w("  m_DynamicOccludee: 1")
    w("  m_StaticShadowCaster: 0")
    w("  m_MotionVectors: 1")
    w("  m_LightProbeUsage: 1")
    w("  m_ReflectionProbeUsage: 1")
    w("  m_RayTracingMode: 2")
    w("  m_RayTraceProcedural: 0")
    w("  m_RenderingLayerMask: 1")
    w("  m_RendererPriority: 0")
    w("  m_Materials:")
    w(f"  - {{fileID: 2100000, guid: {WOOD_MAT_GUID}, type: 2}}")
    w("  m_StaticBatchInfo:")
    w("    firstSubMesh: 0")
    w("    subMeshCount: 0")
    w("  m_StaticBatchRoot: {fileID: 0}")
    w("  m_ProbeAnchor: {fileID: 0}")
    w("  m_LightProbeVolumeOverride: {fileID: 0}")
    w("  m_ScaleInLightmap: 1")
    w("  m_ReceiveGI: 1")
    w("  m_PreserveUVs: 0")
    w("  m_IgnoreNormalsForChartDetection: 0")
    w("  m_ImportantGI: 0")
    w("  m_StitchLightmapSeams: 1")
    w("  m_SelectedEditorRenderState: 3")
    w("  m_MinimumChartSize: 4")
    w("  m_AutoUVMaxDistance: 0.5")
    w("  m_AutoUVMaxAngle: 89")
    w("  m_LightmapParameters: {fileID: 0}")
    w("  m_SortingLayerID: 0")
    w("  m_SortingLayer: 0")
    w("  m_SortingOrder: 0")
    w("  m_AdditionalVertexStreams: {fileID: 0}")
    w("--- !u!65 &130000005")
    w("BoxCollider:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 130000001}")
    w("  m_Material: {fileID: 0}")
    w("  m_IncludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_ExcludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_LayerOverridePriority: 0")
    w("  m_IsTrigger: 0")
    w("  m_ProvidesContacts: 0")
    w("  m_Enabled: 1")
    w("  serializedVersion: 3")
    w("  m_Size: {x: 1, y: 1, z: 1}")
    w("  m_Center: {x: 0, y: 0, z: 0}")

# ============================================================
# Gear (齿轮设置按钮)
# GO=200000001, TF=200000002, BoxCollider=200000003, Rigidbody=200000004,
# CanvasDragHandle=200000005, InteractableUnityEventWrapper=200000006,
# ColliderSurface=200000007, GrabInteractable=200000008
# Child: 设置 TMP text (210)
# ============================================================
def write_gear():
    go_3d(200000001, 200000002, "Gear", layer=0,
           components=[200000003, 200000004, 200000005, 200000006, 200000007, 200000008],
           pos=(0.15, 0.05, 0.5), scale=(0.05, 0.05, 0.05),
           children=[210000002])
    # BoxCollider
    w("--- !u!65 &200000003")
    w("BoxCollider:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 200000001}")
    w("  m_Material: {fileID: 0}")
    w("  m_IncludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_ExcludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_LayerOverridePriority: 0")
    w("  m_IsTrigger: 0")
    w("  m_ProvidesContacts: 0")
    w("  m_Enabled: 1")
    w("  serializedVersion: 3")
    w("  m_Size: {x: 0.094, y: 0.049, z: 0.091}")
    w("  m_Center: {x: 0, y: 0, z: 0}")
    # Rigidbody
    w("--- !u!54 &200000004")
    w("Rigidbody:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 200000001}")
    w("  serializedVersion: 4")
    w("  m_Mass: 1")
    w("  m_Drag: 0")
    w("  m_AngularDrag: 0.05")
    w("  m_CenterOfMass: {x: 0, y: 0, z: 0}")
    w("  m_InertiaTensor: {x: 1, y: 1, z: 1}")
    w("  m_InertiaRotation: {x: 0, y: 0, z: 0, w: 1}")
    w("  m_IncludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_ExcludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_ImplicitCom: 1")
    w("  m_ImplicitTensor: 1")
    w("  m_UseGravity: 0")
    w("  m_IsKinematic: 1")
    w("  m_Interpolate: 0")
    w("  m_Constraints: 0")
    w("  m_CollisionDetection: 0")
    # CanvasDragHandle
    write_monobehaviour(200000005, 200000001, CANVASDRAGHANDLE_GUID,
        f"  mixBord: {{fileID: 0}}\n"
        f"  mixBordGroup: {{fileID: 810000004}}\n"
        f"  bookGroup: {{fileID: 0}}")
    # InteractableUnityEventWrapper — _whenSelect → CanvasDragHandle.ToggleToolPanel()
    write_monobehaviour(200000006, 200000001, INTERACTABLE_GUID,
        f"  _whenSelect:\n"
        f"    m_PersistentCalls:\n"
        f"      m_Calls:\n"
        f"      - m_Target: {{fileID: 200000005}}\n"
        f"        m_TargetAssemblyTypeName: \n"
        f"        m_MethodName: ToggleToolPanel\n"
        f"        m_Mode: 1\n"
        f"        m_Arguments:\n"
        f"          m_ObjectArgument: {{fileID: 0}}\n"
        f"          m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine\n"
        f"          m_IntArgument: 0\n"
        f"          m_FloatArgument: 0\n"
        f"          m_StringArgument: \n"
        f"          m_BoolArgument: 0\n"
        f"        m_CallState: 2")
    # ColliderSurface
    write_monobehaviour(200000007, 200000001, COLLIDERSURFACE_GUID)
    # GrabInteractable
    write_monobehaviour(200000008, 200000001, GRABINTERACTABLE_GUID)
    # 设置 TMP child
    go_3d(210000001, 210000002, "\\u8BBE\\u7F6E", layer=0,
           components=[210000003],
           pos=(0,0,0), rot=(1, -0.27, -0.5, 0), scale=(0.003, 0.003, 0.003),
           father=200000002)
    write_tmp(210000003, 210000001, text="\\u8BBE\\u7F6E", fontsize=36,
              color="0.358,0.022,0.008,1")

# ============================================================
# SimpleGameObject (3D模型容器)
# GO=300000001, TF=300000002, TripoModelLoader=300000003, GltfImport=300000004,
# BoxCollider=300000005, GrabInteractable=300000006, ColliderSurface=300000007,
# InteractableUnityEventWrapper=300000008
# ============================================================
def write_simple_game_object():
    go_3d(300000001, 300000002, "SimpleGameObject", layer=0,
           components=[300000003, 300000004, 300000005, 300000006, 300000007, 300000008],
           pos=(0,0,0), scale=(1,1,1), constrain=1)
    write_monobehaviour(300000003, 300000001, TRIPOMODELLOADER_GUID)
    write_monobehaviour(300000004, 300000001, GLTFIMPORT_GUID)
    # BoxCollider (disabled at start)
    w("--- !u!65 &300000005")
    w("BoxCollider:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 300000001}")
    w("  m_Material: {fileID: 0}")
    w("  m_IncludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_ExcludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_LayerOverridePriority: 0")
    w("  m_IsTrigger: 0")
    w("  m_ProvidesContacts: 0")
    w("  m_Enabled: 0")
    w("  serializedVersion: 3")
    w("  m_Size: {x: 1, y: 1, z: 1}")
    w("  m_Center: {x: 0, y: 0, z: 0}")
    write_monobehaviour(300000006, 300000001, GRABINTERACTABLE_GUID)
    write_monobehaviour(300000007, 300000001, COLLIDERSURFACE_GUID)
    write_monobehaviour(300000008, 300000001, INTERACTABLE_GUID)

# ============================================================
# Camera_Position_W
# GO=400000001, TF=400000002
# ============================================================
def write_camera_position():
    go_3d(400000001, 400000002, "Camera_Position_W", layer=0,
           components=[400000003, 400000004, 400000005],
           pos=(0, 0.1, 0.4), scale=(0.02, 0.02, 0.02))
    # BoxCollider
    w("--- !u!65 &400000003")
    w("BoxCollider:")
    w("  m_ObjectHideFlags: 0")
    w("  m_CorrespondingSourceObject: {fileID: 0}")
    w("  m_PrefabInstance: {fileID: 0}")
    w("  m_PrefabAsset: {fileID: 0}")
    w("  m_GameObject: {fileID: 400000001}")
    w("  m_Material: {fileID: 0}")
    w("  m_IncludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_ExcludeLayers:")
    w("    serializedVersion: 2")
    w("    m_Bits: 0")
    w("  m_LayerOverridePriority: 0")
    w("  m_IsTrigger: 0")
    w("  m_ProvidesContacts: 0")
    w("  m_Enabled: 1")
    w("  serializedVersion: 3")
    w("  m_Size: {x: 1, y: 1, z: 1}")
    w("  m_Center: {x: 0, y: 0, z: 0}")
    write_monobehaviour(400000004, 400000001, GRABINTERACTABLE_GUID)
    write_monobehaviour(400000005, 400000001, COLLIDERSURFACE_GUID)

# ============================================================
# Tripo_Manager
# GO=500000001, TF=500000002, TripoSimpleUI_Manager=500000003, TripoRuntimeCore=500000004
# ============================================================
def write_tripo_manager():
    go_3d(500000001, 500000002, "Tripo_Manager", layer=0,
           components=[500000003, 500000004],
           pos=(0,0,0))
    # TripoSimpleUI_Manager
    write_monobehaviour(500000003, 500000001, TRIPOSIMPLEUI_GUID,
        f"  btnConfirmApiKey: {{fileID: 0}}\n"
        f"  btnTextToModelGenerate: {{fileID: 0}}\n"
        f"  btnLoadImage: {{fileID: 0}}\n"
        f"  btnImageToMdelGenerate: {{fileID: 0}}\n"
        f"  ApiKeyInputField: {{fileID: 0}}\n"
        f"  TextPromptInputField: {{fileID: 0}}\n"
        f"  ImagePathInputField: {{fileID: 0}}\n"
        f"  SimpleModel: {{fileID: 300000001}}\n"
        f"  ModelVersionDropdown: {{fileID: 0}}\n"
        f"  ModelRotationSpeed: 50")
    # TripoRuntimeCore
    write_monobehaviour(500000004, 500000001, TRIPORUNTIME_GUID,
        f"  modelVersion: 4\n"
        f"  face_limit: 8000\n"
        f"  textToModelProgress: 0\n"
        f"  imageToModelProgress: 0\n"
        f"  OnModelGenerateComplete:\n"
        f"    m_PersistentCalls:\n"
        f"      m_Calls:\n"
        f"      - m_Target: {{fileID: 700000003}}\n"
        f"        m_TargetAssemblyTypeName: \n"
        f"        m_MethodName: ChangeBool\n"
        f"        m_Mode: 1\n"
        f"        m_Arguments:\n"
        f"          m_ObjectArgument: {{fileID: 0}}\n"
        f"          m_ObjectArgumentAssemblyTypeName: UnityEngine.Object, UnityEngine\n"
        f"          m_IntArgument: 0\n"
        f"          m_FloatArgument: 0\n"
        f"          m_StringArgument: \n"
        f"          m_BoolArgument: 0\n"
        f"        m_CallState: 2")
