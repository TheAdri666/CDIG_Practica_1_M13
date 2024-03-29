using System;
using System.Linq;
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
        textoDisplay.text = entradaActual;
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
        textoDisplay.text = entradaActual;
    }

    public void PulsarDecimal()
    {
        igualPulsado = false;
        // if (!entradaActual.Contains(".") || entradaActual.Contains(".") && !entradaActual.Substring(entradaActual.IndexOf(".")).Any(c => operadores.Contains(c)))
        if (!puntoEnOperador)
        {
            puntoEnOperador = true;
            entradaActual += ".";
            textoDisplay.text = entradaActual;
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
        textoDisplay.text = entradaActual;
    }

    public void PulsarRetroceso()
    {
        if (igualPulsado)
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
            textoDisplay.text = entradaActual;
        }
    }

    public void PulsarMemoriaSuma()
    {
        resultadoMemoria += Convert.ToDouble(entradaActual);
    }

    public void PulsarMemoriaResta()
    {
        resultadoMemoria -= Convert.ToDouble(entradaActual);
    }

    public void PulsarMemoriaLimpiar()
    {
        resultadoMemoria = 0;
    }

    public void PulsarMemoriaRecuperar()
    {
        entradaActual = resultadoMemoria.ToString();
        textoDisplay.text = entradaActual;
    }

    private void Calcular()
    {
        if (operadores.Contains(entradaActual[entradaActual.Length - 1]))
        {
            entradaActual = entradaActual.Remove(entradaActual.Length - 1);
        }

        try
        {
            var resultado = Convert.ToDouble(new System.Data.DataTable().Compute(entradaActual, ""));
            entradaActual = resultado.ToString("G5");
            textoDisplay.text = entradaActual;
        }
        catch (Exception e)
        {
            Debug.LogError("Error al calcular el resultado: " + e.Message);
        }
    }

}
