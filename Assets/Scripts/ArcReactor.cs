using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ArcReactor : MonoBehaviour
{
    // if the reactor had ~110g total of material that will be used for fusion,
    // then it will have a total energy capacity of ~10petaJoules which is around 2.77 TeraWatt hours.

    public float maxFuel; // Grams of material to be used for fusion

    float fuel; // grams

    public float getFuelGrams()
    {
        return fuel;
    }

    public float getFuelKiloWattHours()
    {
        return PhysMath.gramsToKiloWattHours(fuel);
    }

    public float getFuelKiloJoules()
    {
        return PhysMath.gramsToKiloJoules(fuel);
    }

    private void deductFuel(float amount)
    {
        fuel -= amount;
        if (amount < 0)
        {
            amount = 0;
            // We hit zero fuel!
        }
    }
    public void deductGrams(float amount)
    {
        deductFuel(amount);
    }

    public void deductKiloWattHours(float amount)
    {
        deductFuel(PhysMath.kiloWattHoursToGrams(amount));
    }

    public void deductKiloWattSeconds(float amount)
    {
        deductFuel(PhysMath.kiloWattHoursToGrams(PhysMath.secondsToHours(amount)));
    }

    public void deductKiloJoules(float amount)
    {
        deductFuel(PhysMath.kiloJoulesToGrams(amount));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fuel = maxFuel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
