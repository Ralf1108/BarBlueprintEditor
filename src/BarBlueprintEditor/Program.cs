namespace BarBlueprintEditor
{
    /// <summary>
    /// unit names: https://github.com/beyond-all-reason/Beyond-All-Reason/tree/master/units
    /// images: https://www.beyondallreason.info/units/armada-defense-buildings
    /// blueprint json: C:\Games\Beyond-All-Reason\data\LuaUI\Config\blueprints.json
    ///
    /// Canvas:
    /// Pan/Zoom: https://github.com/shaigem/BlazorPanzoom
    /// </summary>
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var blueprintsFilePath = @"D:\Projects\BarBlueprintEditor\Testdata\blueprints.json";
            var unitFilePath = @"D:\Projects\BarBlueprintEditor\Testdata\armflak.lua";
            var blueprints = Tools.ParseFromBlueprints(File.ReadAllText(blueprintsFilePath));
            var unitDefinition = Tools.ParseFromLuaDefinition(File.ReadAllText(unitFilePath));

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}