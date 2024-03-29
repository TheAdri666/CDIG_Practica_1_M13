using System;
using System.Linq;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculatorManager : MonoBehaviour
{

    public Text textoDisplay;

    private string entradaActual = "0";
    private double resultadoMemoria = 0;
    private bool igualPulsado = false;
    private bool puntoEnOperador = false;
    private char[] operadores = { '+', '-', '*', '/' };

    private const int MAX_CARACTERES_MOSTRADOS = 10;

    public void PulsarNumero(string digito)
    {
        if (entradaActual == "0" || igualPulsado)
        {
            entradaActual = digito;
            igualPulsado = false;
        }
        else
        {
            entradaActual += digito;
        }
        ActualizarTextoDisplay();
    }

    public void PulsarOperacion(string operacion)
    {
        igualPulsado = false;
        puntoEnOperador = false;
        if (entradaActual.Any(operadores.Contains))
        {
            Calcular();
        }
        entradaActual += operacion;
        ActualizarTextoDisplay();
    }

    public void PulsarDecimal()
    {
        igualPulsado = false;
        if (!puntoEnOperador)
        {
            puntoEnOperador = true;
            entradaActual += ".";
            ActualizarTextoDisplay();
        }
    }

    public void PulsarIgual()
    {
        Calcular();
        igualPulsado = true;
        puntoEnOperador = false;
    }

    public void PulsarBorrar()
    {
        igualPulsado = false;
        puntoEnOperador = false;
        entradaActual = "0";
        ActualizarTextoDisplay();
    }

    public void PulsarRetroceso()
    {
        if (igualPulsado || UltimoCaracterEsOperador())
        {
            PulsarBorrar();
        }
        else
        {
            if (entradaActual.Length > 1)
            {
                if (entradaActual[entradaActual.Length - 1] == '.')
                {
                    puntoEnOperador = false;
                }
                entradaActual = entradaActual.Remove(entradaActual.Length - 1);
            }
            else
            {
                entradaActual = "0";
            }
            ActualizarTextoDisplay();
        }
    }

    public void PulsarMemoriaSuma()
    {
        resultadoMemoria += Convert.ToDouble(entradaActual, CultureInfo.InvariantCulture);
    }

    public void PulsarMemoriaResta()
    {
        resultadoMemoria -= Convert.ToDouble(entradaActual, CultureInfo.InvariantCulture);
    }

    public void PulsarMemoriaLimpiar()
    {
        resultadoMemoria = 0;
    }

    public void PulsarMemoriaRecuperar()
    {
        entradaActual = resultadoMemoria.ToString(CultureInfo.InvariantCulture);
        if (entradaActual.Contains('.'))
        {
            puntoEnOperador = true;
        }
        ActualizarTextoDisplay();
    }

    private void Calcular()
    {
        if (UltimoCaracterEsOperador())
        {
            entradaActual = entradaActual.Remove(entradaActual.Length - 1);
        }

        try
        {
            var resultado = Convert.ToDouble(new System.Data.DataTable().Compute(entradaActual, ""));
            entradaActual = resultado.ToString("G8", CultureInfo.InvariantCulture);
            ActualizarTextoDisplay();
        }
        catch (Exception e)
        {
            Debug.LogError("Error al calcular el resultado: " + e.Message);
        }
    }

    private void ActualizarTextoDisplay()
    {
        if (entradaActual.Length > MAX_CARACTERES_MOSTRADOS)
        {
            textoDisplay.text = entradaActual.Substring(entradaActual.Length - MAX_CARACTERES_MOSTRADOS);
        }
        else
        {
            textoDisplay.text = entradaActual;
        }
    }

    private bool UltimoCaracterEsOperador()
    {
        return operadores.Contains(entradaActual[entradaActual.Length - 1]);
    }
}
