using UnityEngine;

/// <summary>
/// [Owner: E - Trung]
/// ScriptableObject chua toan bo so lieu balance cua game.
/// Chi la DATA – khong co logic. Cac system khac doc qua so reference.
/// De chinh so: click GoldEconomyConfig.asset – sua Inspector – tu dong luu.
/// Khong can rebuild khi chinh so.
/// </summary>

[CreateAssetMenu(
    fileName = "GoldEconomyConfig",
    menuName = "FactoryRush/GoldEconomyConfig")]
public class GoldEconomyConfigSO : ScriptableObject
{

    // ================================================================
    // GIA BAN ITEM (vang)
    // Dung boi: MarketManager (Member B)
    // ================================================================

    [Header("Gia Ban Item (vang)")]

    [Tooltip("Banh mi = Lua mi x2 | San xuat 25s")]
    public int breadSellPrice = 30;

    [Tooltip("Trung chien = Trung x2 | San xuat 20s")]
    public int friedEggSellPrice = 25;

    [Tooltip("Banh ngot = Bot mi + Trung | San xuat 40s")]
    public int cakeSellPrice = 80;


    // ================================================================
    // GIA MUA BUILDING (vang)
    // Dung boi: BuildingUnlockSystem (Member C)
    // ================================================================

    [Header("Gia Mua Building (vang)")]

    [Tooltip("Bep nau – unlock de san xuat Trung chien 25v")]
    public int kitchenBuyPrice = 60;

    [Tooltip("May xay – unlock de san xuat Bot mi")]
    public int millBuyPrice = 80;

    [Tooltip("Lo banh ngot – unlock de san xuat Banh ngot 80v")]
    public int cakeOvenBuyPrice = 150;


    // ================================================================
    // CHI PHI GIA HAN THOI GIAN (vang)
    // Dung boi: TimeExtensionSystem (Member D)
    // ================================================================

    [Header("Chi Phi Gia Han Thoi Gian (vang)")]

    [Tooltip("+1 phut | Co loi neu kiem duoc > 50v trong 1 phut tiep theo")]
    public int extend1MinCost = 50;

    [Tooltip("+3 phut | Tiet kiem hon nhung rui ro cao hon")]
    public int extend3MinCost = 120;

    [Tooltip("+5 phut | Chi co loi neu layout cuc ky tot")]
    public int extend5MinCost = 180;

    [Tooltip("Toi da so lan gia han / luot – requirement: 2")]
    public int maxExtensionCount = 2;


    // ================================================================
    // THOI GIAN SAN XUAT (giay)
    // Dung boi: MachineController (Member A)
    // ================================================================

    [Header("Thoi Gian San Xuat (giay)")]

    public float wheatFieldTime = 15f;   // Canh dong lua
    public float chickenCoopTime = 20f;  // Chuong ga
    public float bakeryTime = 25f;       // Lo banh mi
    public float kitchenTime = 20f;      // Bep nau
    public float millTime = 30f;         // May xay
    public float cakeOvenTime = 40f;     // Lo banh ngot


    // ================================================================
    // GAME CONFIG
    // Dung boi: TimerManager + GameStateManager (Member D)
    // ================================================================

    [Header("Game Config")]

    [Tooltip("Thoi gian moi luot (giay). 300 = 5 phut theo requirement")]
    public int gameDurationSeconds = 300;

    [Tooltip("Vang nguoi choi co khi bat dau luot")]
    public int startingGold = 0;

}