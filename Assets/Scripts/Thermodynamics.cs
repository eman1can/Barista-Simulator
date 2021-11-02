using UnityEngine;
/*
 * Created by Ethan Wolfe
 * A heavily math involved thermodynamics utility class
 * Not currently implemented
 * See individual comments to see referecne sources
 */
public class Thermodynamics : MonoBehaviour {
    private float ToKelvin(float c) {
        return c + 273.15f;
    }


    // All heat transfer is a derivative of
    // q = hAdT, where q is Heat lost in W/s
    // A, is the surface area, and dT is the delta temperature
    private float CalculateEnergy(float h, float sa, float dt) {
        return h * sa * dt;
    }

    // Conductive heat transfer
    // https://www.nuclear-power.com/nuclear-engineering/heat-transfer/thermal-conduction/thermal-conductivity/thermal-conductivity-of-glass/
    // https://www.engineeringtoolbox.com/conductive-heat-transfer-d_428.html

    // Thermal Capacity, k = 0.96 W/m°K
    // Heat Coefficient, h = (k / s)
    // Surface Area, SA = 0.06 m^2
    // Delta Temperature, dT = 70°C
    // Thickness, s = 0.05 m
    // q = hSAdT W/s

    // private float thermalConductivityOfGlass = 0.96f;

    public float ConductionHeatTransfer(float k, float sa, float dt, float s) {
        float h = k / s;
        return CalculateEnergy(h, sa, dt);
    }

    // Radiation heat transfer (loss)
    // https://www.engineeringtoolbox.com/radiation-heat-transfer-d_431.html
    // σ = 5.6703 10-8 (W/m2K4) Stefan-Boltzmann Constant
    // Absolute Temperature, T = 90 C + 
    // Surface Area, SA = 0.06 m^2
    // q = εσT^4 A
    float stefanBoltzmannConstant = 5.6703f * Mathf.Pow(10.0f, -8.0f);
    private float emissivityL = 0.95f;
    private float emissivityH = 0.963f;

    public float RadiationHeatTransfer(float t, float a) {
        float emissivity = Random.Range(emissivityL, emissivityH);
        float T = Mathf.Pow(ToKelvin(t), 4);
        return emissivity * stefanBoltzmannConstant * T * a;
    }

    // Math reference
    // https://aip.scitation.org/doi/pdf/10.1063/5.0041799
    // https://mathbitsnotebook.com/Algebra2/Exponential/EXCooling.html
    // http://jwilson.coe.uga.edu/EMAT6680Fa2014/Gieseking/Exploration%2012/Newton%27s%20Law%20of%20Cooling.htm
    // https://en.wikipedia.org/wiki/Newton%27s_law_of_cooling
    // https://en.wikipedia.org/wiki/Heat_transfer_coefficient
    // https://www.engineeringtoolbox.com/convective-heat-transfer-d_430.html

    // https://digital.library.unt.edu/ark:/67531/metadc177190/m2/1/high_res_d/thesis.pdf
    // https://s3.amazonaws.com/suncam/docs/119.pdf
    // https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.1024.458&rep=rep1&type=pdf
    // https://courses.lumenlearning.com/physics/chapter/13-2-thermal-expansion-of-solids-and-liquids/
    // https://en.wikipedia.org/wiki/List_of_thermal_conductivities
    // https://en.wikipedia.org/wiki/Natural_convection
    // https://www.engineersedge.com/heat_transfer/convection.htm
    // https://link-springer-com.ezproxy.csuci.edu/content/pdf/10.1007/s12206-015-0952-x.pdf
    // https://pdf.sciencedirectassets.com/271641/1-s2.0-S1359431120X00202/1-s2.0-S1359431120339156/main.pdf?X-Amz-Security-Token=IQoJb3JpZ2luX2VjEC0aCXVzLWVhc3QtMSJIMEYCIQDzkiTYXa0jtpiPs3LyElMcf%2FgIiPAG5%2FV9rAOUefxG4AIhALYI%2F1rSb9N%2BbkpDJIVqQVN%2BO8HfR2fzlC5KRLOytBiYKoMECLX%2F%2F%2F%2F%2F%2F%2F%2F%2F%2FwEQBBoMMDU5MDAzNTQ2ODY1IgxShRwsJcID1itllEkq1wOIS4soCqUh5J1pMo7b3%2B1%2BUw4tVqM3hsUSBkrGOFeaxh6dzLIsjJYpBoNY4VaIsxlWKuDHUU8Abxyv6%2BVLBLgJ1qjsP3e8aKeveesyee2oumgeUiFUGtclfQIQbkj71uSk834wh3pF9cxbp5dc9W3xSYENClE5wcUKu9Uwe64YqkbH2ifFfnYopy7%2FW3hDJ8uq8CKnpG31bB8pI4WTvfTOvddPFTlZ4lOOkY52LQD4Ouk9TQQPzfCnV01Cysz3Vlq1fFxK8CQqVSI9%2BjC0A%2FwUq7Q2xec0OOds4pBYQo2EKU5KUiv%2BwlctAi2OFGbpaTKm%2FVjjbhw4vIyiyQ220THzcZGtpJf03AmfOh43OQNTUPs5U%2BlARx0v%2FyYgGy4Wcjy2jdK%2B1KYLCt53R3gjpwLzygFvJ%2BJM7ShVnBPrByh7L6YHHtG3qodenhkmWbtgdsgULJzpJaVuBS%2BnDYYWbclEmhgFXSnpK0ldbV5IZSd8zeISUNpguNK2B7NGPC2yl0lGrzkS3fqFcSgwIiTVlDILoztEDpzpyjT4Oa0IncQZ6n5rW0k8cNdWpRxYqw0TGviMGq4jMbrOdlQIcwVvPqTf6a6rjmV208945PXKp32Ze0LgtAqGxxcwwYi5iwY6pAGz2SF2BOppWNL9COufNCIdtwm9QyDIJKDe0Ti1LNFO7Ab%2BXi1qfPvCSWvv9XhovM2S9c%2Ba9U2kZFp5ifqntEeKyQJHmYRYUZsksKoqiru2ZlehbH1CNvsB3Sn1UWql9zmxGU8epqPUmadlI46JexzJpNemLINHw4MF70yU1sx2EF9T14WTaT96UP1ItfN6PQxIAIMJF6%2FPhfkddL9PPXOBtaU1HQ%3D%3D&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Date=20211019T060230Z&X-Amz-SignedHeaders=host&X-Amz-Expires=300&X-Amz-Credential=ASIAQ3PHCVTYZJLLGQN6%2F20211019%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Signature=9ab19605fb46196552ffac842311788843dc005e9498d89fe5c61512a6946e87&hash=3acf75c5110326e1b8257f4955adac81164bfdaae92cdefb83fa79760905f20c&host=68042c943591013ac2b2430a89b270f6af2c76d8dfd086a07176afe7c76c2c61&pii=S1359431120339156&tid=spdf-ece4f480-2bd5-4581-83b3-c78e1d6bf74d&sid=19fe7e415daeb546ef9b1545d3aaddeb35c2gxrqa&type=client
    // https://link-springer-com.ezproxy.csuci.edu/content/pdf/10.1007%2F978-3-319-08132-8.pdf
    // https://en.wikipedia.org/wiki/Nusselt_number

    // Assume Water (Close enough)
    // Temperature Coffee, T1 = 90 °C
    // Temperature Ambient, T2 = 20 °C

    private float T1 = 90f;
    private float T2 = 20f;

    // Volume Expansion Coefficient, β = 210 × 10 ^ -6 1/°C
    // Delta Temperature, dT = |T2 - T1| = 70°
    // Height, h = 0.49 * 0.75 = 0.03675 m
    // Volume, V = 0.195 * 0.295 * h = 0.002114 m^3

    private float B = 210f * Mathf.Pow(10f, -6f);
    //private float dT = Mathf.Abs(T2 - T1);

    // Delta Volume = βVdT = 210 × 10 ^ -6 * 0.002114 * 70 = 0.000031076 m^3
    // Expanded Size = 0.000031076 + 0.002114 = 0.002145076 m^3
    private float ExpandedVolume(float B, float V, float dT) {
        return B * V * dT;
    }

    // Mass, m = 0.001 kg
    // Density, ρ = m / V = 5.16 kg/m³

    // Viscosity Constant, µA = 2.414 × 10^−5
    // Viscosity Constant, µB = 247.8 K
    // Viscosity Constant, µC = 140 K
    // Viscosity, µ = µA * 10 ^ (µB / (T1 + 273.15 − µC)) Pa·s = 0.000311

    // Specific Heat, Cp = 4.19 kJ/KgK
    // Thermal Capacity, k = 0.5918 W/mk
    // Surface Area, SA = 0.2 * 0.3 = 0.06 m^2 (Only the top of the container)
    // Perimeter, P = 0.2 * 2 + 0.3 * 2 = 1 m
    // Characteristic Length = SA / P = 0.06 / 1 = 0.06 m
    // Gravity, g = 9.81 m/s^2

    // Prandtl number, Pr = µ * 1000 * Cp / k = 0.000311 * 4.19 / 0.5918 = 2.2019
    // Grashof number, Gr = D^3ρ^2gdTβ/µ^2
    //                 Gr = 0.06 ^ 3 * 5.16 ^ 2 * 9.81 * 70 * 210 * 10 ^ -6 / 0.000311 ^ 2
    //                 Gr = 18880.62
    // Rayleigh number, Ra = Gr * Pr = 2.2019 * 8574.6957 = 18.88
    // Nusselt number, Nu = (0.60 + (0.387 * Ra ^ 1/6) / (1 + (0.559 / Pr) ^ 9/16) ^ 8/27) ^ 2
    //                 Nu = (0.60 + (0.387 * (18880.62)^(1/6)) / (1 + (0.559 / 2.2019)^(9/16))^(8/27))^2
    //                 Nu = 5.6848
    // Heat Transfer Coefficient, h = Nu * k / D = 5.6848 * 0.5918 / 0.06 = 56.07
    // Heat Loss = hSAdT = 56.07 * 0.06 * 70

    private float Prandtl(float u, float Cp, float k) {
        return u * 1000f * Cp / k;
    }

    private const float g = 9.81f;

    private float Grashof(float D, float p, float dT, float B, float u) {
        // D^3ρ^2gdTβ/µ^2
        return Mathf.Pow(D, 3) * Mathf.Pow(p, 2) * g * dT * B / Mathf.Pow(u, 2);
    }

    private float Rayleigh(float Gr, float Pr) {
        return Gr * Pr;
    }

    // Heat loss for rectangle - This is the combination of the different walls and the bottom
    // Equation taken from https://en.wikipedia.org/wiki/Nusselt_number#Free_convection_at_a_vertical_wall
    // and https://en.wikipedia.org/wiki/Nusselt_number#Free_convection_from_horizontal_plates
    // Calculate for the bottom
    // A rough estimate for characteristic Length is V / SA or SA / P
    // Characteristic Length L = SA / P = 0.06
    // Nu = 0.52 Ra ^ 1/5 (10^5 <= Ra <= 10^10
    // Heat Transfer Coefficient, h = Nu * k / L = 5.6848 * 0.5918 / 0.06 = 56.07

    // 0.54 for 10^4 <= RaL <= 10^7
    // 0.15 for 10^7 <= RaL <= 10^11
    private float NusseltRectPlaneOut(float Ra) {
        return 0.54f * Mathf.Pow(Ra, 1 / 5f);
    }

    // 0.52 for 10^5 <= RaL <= 10^10
    private float NusseltRectPlaneIn(float Ra) {
        return 0.52f * Mathf.Pow(Ra, 1 / 5f);
    }

    // RaL <= 10^8
    private float NusseltRectWall(float Ra, float Pr) {
        return 0.68f + 0.663f * Mathf.Pow(Ra, 1 / 4f) / Mathf.Pow(1 + Mathf.Pow(0.492f / Pr, 9 / 16f), 4 / 9f);
    }

    private float HeatCoefficient(float Nu, float k, float D) {
        return Nu * k / D;
    }

    // k = 0.5918
    private float ConvectionCuboid(Liquid liquid, float width, float height, float length, float ambient) {
        float V = width * height * length;
        
        float dT = Mathf.Abs(ambient - liquid.GetTemperature());
        float k = liquid.GetSpecificHeat();
        float Cp = liquid.GetHeatCapacity();
        float p = liquid.GetDensity(V);
        float u = liquid.GetViscosity();

        float Pr = Prandtl(u, Cp, k);
        
        // Top and Bottom
        float SAt = width * length;
        float Pt = width * 2f + length * 2f;
        float Dt = SAt / Pt;
        
        float Grt = Grashof(Dt, p, dT, B, u);
        float Rat = Rayleigh(Grt, Pr);
        float Nut;
        if (dT > 0)
            Nut = NusseltRectPlaneIn(Rat);
        else
            Nut = NusseltRectPlaneOut(Rat);
        float ht = 2f * HeatCoefficient(Nut, k, Dt);
        float q1 = CalculateEnergy(ht, SAt * 2f, dT);
        
        // Side 1 and 3
        float SAs1 = width * height;
        float Ds1 = V / SAs1;
        float Grs1 = Grashof(Ds1, p, dT, B, u);
        float Ras1 = Rayleigh(Grs1, Pr);
        float Nus1;
        if (dT > 0)
            Nus1 = NusseltRectWall(Ras1, Pr);
        else
            Nus1 = NusseltRectWall(Ras1, Pr);
        float hs1 = 2f * HeatCoefficient(Nus1, k, Dt);
        float q2 = CalculateEnergy(hs1, SAs1 * 2f, dT);
        
        // Side 2 and 4
        float SAs2 = width * height;
        float Ds2 = V / SAs2;
        float Grs2 = Grashof(Ds2, p, dT, B, u);
        float Ras2 = Rayleigh(Grs2, Pr);
        float Nus2;
        if (dT > 0)
            Nus2 = NusseltRectWall(Ras2, Pr);
        else
            Nus2 = NusseltRectWall(Ras2, Pr);
        float hs2 = 2f * HeatCoefficient(Nus2, k, Dt);
        float q3 = CalculateEnergy(hs2, SAs2 * 2f, dT);

        return q1 + q2 + q3;
    }
    // Heat loss for a Cylinder
    
    // Aspect Ratio, AR = L / D
    // 
    // For 0.1 ≤ AR ≤ 1
    // Nusselt number, Nu = -0.2165 + 0.5204 * Ra^ (1/4) + 0.8473 * AR
    // For 2 ≤ AR ≤ 10
    // Nusselt number, Nu = -0.6211 + 0.54414 * Ra^ (1/4) + 0.6123 * AR
    
    public float ConvectionHeatTransfer(float temperatureOne, float temperatureTwo, float area) {
        
        
        
        float heatTransferCoefficient = 0f;
        return heatTransferCoefficient * area * Mathf.Abs(temperatureTwo - temperatureOne);
    }
}
