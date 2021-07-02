using Godot;
using static Godot.GD;
using System;

public class MainScreen : Node
{
    public string X509CertificateName => "X509.crt";
    public string X509CertificatePath => "user://Certificate";
    public string X509CertificateFullPath => $"{X509CertificatePath}/{X509CertificateName}";

    public string X509KeyName => "X509.key";
    public string X509KeyPath => X509CertificatePath;
    public string X509KeyFullPath => $"{X509KeyPath}/{X509KeyName}";

    private LineEdit CN { get; set; }
    private LineEdit O { get; set; }
    private LineEdit C { get; set; }
    private LineEdit NotBefore { get; set; }
    private LineEdit NotAfter { get; set; }
    
    private Label   SeedValue  { get; set; }
    private HSlider SeedSlider { get; set; }
    
    private Label   OctavesValue  { get; set; }
    private HSlider OctavesSlider { get; set; }
    
    private Label   PeriodValue  { get; set; }
    private HSlider PeriodSlider { get; set; }
    
    private Label   PersistanceValue  { get; set; }
    private HSlider PersistanceSlider { get; set; }
    
    private Label   LacunarityValue  { get; set; }
    private HSlider LacunaritySlider { get; set; }

    private Label   SizeValue  { get; set; }
    private HSlider SizeSlider { get; set; }
    
    public override void _Ready()
    {
        base._Ready();
        
        CN = GetNode<LineEdit>("Tools/GenerateX509CertificateContainer/Pnl_CN/Txt_CN");
        O  = GetNode<LineEdit>("Tools/GenerateX509CertificateContainer/Pnl_O/Txt_O");
        C  = GetNode<LineEdit>("Tools/GenerateX509CertificateContainer/Pnl_C/Txt_C");
        
        NotBefore  = GetNode<LineEdit>("Tools/GenerateX509CertificateContainer/Pnl_NotBefore/Txt_NotBefore");
        NotAfter   = GetNode<LineEdit>("Tools/GenerateX509CertificateContainer/Pnl_NotAfter/Txt_NotAfter");

        var dt = DateTime.Now;
        NotBefore.Text = $"{dt:yyyyMMdd}000000";
        NotAfter.Text = $"{dt.AddYears(1):yyyyMMdd}235900";

        var noiseGenerationToolRoot = "Tools/NoiseGeneratorContainer";

        OctavesValue  = GetNode<Label>(  $"{noiseGenerationToolRoot}/HBoxOctaves/VBoxContainer2/LblOctavesValue");
        OctavesSlider = GetNode<HSlider>($"{noiseGenerationToolRoot}/HBoxOctaves/VBoxContainer/HSliderOctaves");
        OctavesValue.Text = Math.Round(OctavesSlider.Value, 3).ToString();

        PeriodValue  = GetNode<Label>(  $"{noiseGenerationToolRoot}/HBoxPeriod/VBoxContainer2/LblPeriodValue");
        PeriodSlider = GetNode<HSlider>($"{noiseGenerationToolRoot}/HBoxPeriod/VBoxContainer/HSliderPeriod");
        PeriodValue.Text = Math.Round(PeriodSlider.Value, 3).ToString();
        
        PersistanceValue  = GetNode<Label>(  $"{noiseGenerationToolRoot}/HBoxPersistance/VBoxContainer2/LblPersistanceValue");
        PersistanceSlider = GetNode<HSlider>($"{noiseGenerationToolRoot}/HBoxPersistance/VBoxContainer/HSliderPersistance");
        PersistanceValue.Text = Math.Round(PersistanceSlider.Value, 3).ToString();
        
        LacunarityValue  = GetNode<Label>(  $"{noiseGenerationToolRoot}/HBoxLacunarity/VBoxContainer2/LblLacunarityValue");
        LacunaritySlider = GetNode<HSlider>($"{noiseGenerationToolRoot}/HBoxLacunarity/VBoxContainer/HSliderLacunarity");
        LacunarityValue.Text = Math.Round(LacunaritySlider.Value, 3).ToString();
        
        SizeValue  = GetNode<Label>(  $"{noiseGenerationToolRoot}/HBoxSize/VBoxContainer2/LblSizeValue");
        SizeSlider = GetNode<HSlider>($"{noiseGenerationToolRoot}/HBoxSize/VBoxContainer/HSliderSize");
        SizeValue.Text = Math.Round(SizeSlider.Value, 3).ToString();
        
        SeedValue  = GetNode<Label>(  $"{noiseGenerationToolRoot}/HBoxSeed/VBoxContainer2/LblSeedValue");
        SeedSlider = GetNode<HSlider>($"{noiseGenerationToolRoot}/HBoxSeed/VBoxContainer/HSliderSeed");
        SeedValue.Text = Math.Round(SeedSlider.Value, 3).ToString();
    }

    public void OnGenerateX509Certificate()
    {
        Print($"Generating X509 Certificate: \n> CN:{CN.Text}, O:{O.Text}, C:{C.Text}, \n> Not Before:{NotBefore.Text}, Not After:{NotAfter.Text}");

        var saveDirectory = new Directory();
        if (!saveDirectory.DirExists(X509CertificatePath))
        {
            saveDirectory.MakeDir(X509CertificatePath);
        }
        
        Generate();
        Print("> Finished");
    }

    public void HSliderSeedValueChanged(float value)
    {
        SeedValue.Text = Math.Round(value, 3).ToString();
    }
    
    public void HSliderOctavesValueChanged(float value)
    {
        OctavesValue.Text = Math.Round(value, 3).ToString();
    }
    
    public void HSliderPeriodValueChanged(float value)
    {
        PeriodValue.Text = Math.Round(value, 3).ToString();
    }
    
    public void HSliderPersistanceValueChanged(float value)
    {
        PersistanceValue.Text = Math.Round(value, 3).ToString();
    }
    
    public void HSliderLacunarityValueChanged(float value)
    {
        LacunarityValue.Text = Math.Round(value, 3).ToString();
    }
    
    public void HSliderSizeValueChanged(float value)
    {
        SizeValue.Text = Math.Round(value, 3).ToString();
    }

    public void OnSaveButtonPressed()
    {
        var noise = new OpenSimplexNoise();
        // noise.Seed = new Random().Next(1000000);

        noise.Seed = (int) SeedSlider.Value;
        noise.Octaves = (int) OctavesSlider.Value;
        noise.Period = (float) PeriodSlider.Value;
        noise.Persistence = (float) PersistanceSlider.Value;
        noise.Lacunarity = (float) LacunaritySlider.Value;
        
        var image = noise.GetSeamlessImage((int)SizeSlider.Value);
        var path = "user://noise.png";
        image.SavePng(path);
    }

    private void Generate()
    {
        var CN_O_C = $"CN={CN.Text},O={O.Text},C={C.Text}";
        var crypto = new Crypto();
        var key = crypto.GenerateRsa(4096);
        var cert = crypto.GenerateSelfSignedCertificate(key, CN_O_C, NotBefore.Text, NotAfter.Text);
        cert.Save(X509CertificateFullPath);
        key.Save(X509KeyFullPath);
    }
}
