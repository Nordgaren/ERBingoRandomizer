﻿using FSParam;

namespace Project.Params; 

public class AtkParam {
    private Param.Cell _spEffectAtkPowerCorrectRate_byPoint;
    private Param.Cell _spEffectAtkPowerCorrectRate_byRate;
    private Param.Cell _spEffectAtkPowerCorrectRate_byDmg;

    public AtkParam(Param.Row atk) {
        _spEffectAtkPowerCorrectRate_byPoint = atk["spEffectAtkPowerCorrectRate_byPoint"]!.Value;
        _spEffectAtkPowerCorrectRate_byRate = atk["spEffectAtkPowerCorrectRate_byRate"]!.Value;
        _spEffectAtkPowerCorrectRate_byDmg = atk["spEffectAtkPowerCorrectRate_byDmg"]!.Value;
    }
    public ushort spEffectAtkPowerCorrectRate_byPoint { get => (ushort)_spEffectAtkPowerCorrectRate_byPoint.Value; set => _spEffectAtkPowerCorrectRate_byPoint.Value = value; }
    public ushort spEffectAtkPowerCorrectRate_byRate { get => (ushort)_spEffectAtkPowerCorrectRate_byRate.Value; set => _spEffectAtkPowerCorrectRate_byRate.Value = value; }
    public ushort spEffectAtkPowerCorrectRate_byDmg { get => (ushort)_spEffectAtkPowerCorrectRate_byDmg.Value; set => _spEffectAtkPowerCorrectRate_byDmg.Value = value; }
}
