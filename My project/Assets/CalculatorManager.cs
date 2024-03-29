using System;
using System.Linq;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculatorManager : MonoBehaviour
{

    //Variables con la inicialización debida o indicada en el enunciado
    public Text textoDisplay;
    public Button memRecoverBtn;
    private string entradaActual = "0";
    private string resultadoMemoria = "0";
    private bool igualPulsado = false;
    private bool puntoEnOperador = false;
    private char[] operadores = { '+', '-', '*', '/' };

    private const int MAX_CARACTERES_MOSTRADOS = 10;

    //Al pulsar un número de la calculadorá, se almacena para su futuro calculo y se actualiza la expresión mostrada por pantalla
    public void PulsarNumero(string digito)
    {
        //Se diferencia si es el primer dígito o si ya se ha introducido alguno
        if (entradaActual == "0" || igualPulsado)
        {
            entradaActual = digito;

            //Booleanos utiles para el funcionamiento de la calculadora
            igualPulsado = false;
            puntoEnOperador = false;
        }
        else
            entradaActual += digito;

        ActualizarTextoDisplay();
    }

    public void PulsarOperacion(string operacion)
    {
        igualPulsado = false;
        puntoEnOperador = false;

        //Si en vez de pulsar igual (=), sigues haciendo operaciones, la calculadora te calculará el resultado de esa operación primera
        if (entradaActual.Any(operadores.Contains))
            Calcular();

        entradaActual += operacion;
        ActualizarTextoDisplay();
    }
    //Si se ha pulsado un decimal, se actualiza por pantalla correspondientemente
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
    //Se procederá a calcular la expresión tras pulsar el botón "="
    public void PulsarIgual()
    {
        Calcular();
        igualPulsado = true;
    }
    //Si borramos todo el contenido, volverá a su estado inicial (0)
    public void PulsarBorrar()
    {
        igualPulsado = false;
        puntoEnOperador = false;
        entradaActual = "0";
        ActualizarTextoDisplay();
    }

    public void PulsarRetroceso()
    {
        //Si el último carácter fue un = o un operador, se borrará toda la expresión
        if (igualPulsado || UltimoCaracterEsOperador())
            PulsarBorrar();
        else
        {
            //Se comprueba que la expresión tenga algún digito, sino se deja en el estado inicial (0)
            if (entradaActual.Length > 1)
            {
                if (entradaActual[entradaActual.Length - 1] == '.')
                    puntoEnOperador = false;

                entradaActual = entradaActual.Remove(entradaActual.Length - 1);
            }
            else
                entradaActual = "0";

            ActualizarTextoDisplay();
        }
    }

    //Aquí adelante son las funciones que actualizan la expresión con lo que haya en memoria
    //Aquí se le suma a lo que haya en memoría, la expresión actual que haya introducido el usuario, y se guarda en memoría
    public void PulsarMemoriaSuma()
    {
        double resultado = Convert.ToDouble(resultadoMemoria, CultureInfo.InvariantCulture) + Convert.ToDouble(ParsearResultado(entradaActual), CultureInfo.InvariantCulture);
        resultadoMemoria = ParsearResultado(resultado.ToString());
        PulsarMemoriaRecuperar();
    }
    //Aquí se le resta a lo que haya en memoría, la expresión actual que haya introducido el usuario, y se guarda en memoría
    public void PulsarMemoriaResta()
    {
        double resultado = Convert.ToDouble(resultadoMemoria, CultureInfo.InvariantCulture) - Convert.ToDouble(ParsearResultado(entradaActual), CultureInfo.InvariantCulture);
        resultadoMemoria = ParsearResultado(resultado.ToString());
        PulsarMemoriaRecuperar();
    }
    //El valor que habrá en memoria se resetea al inicial (0)
    public void PulsarMemoriaLimpiar()
    {
        resultadoMemoria = "0";
    }
    //La expresión que se mostrará por pantalla, es el número guardado en memoría
    public void PulsarMemoriaRecuperar()
    {
        entradaActual = resultadoMemoria.ToString(CultureInfo.InvariantCulture);
        if (entradaActual.Contains('.'))
            puntoEnOperador = true;

        ActualizarTextoDisplay();
    }


    //Calcula el resultado de la expresión y lo actualiza por pantalla
    private void Calcular()
    {
        //Si en vez de pulsar igual (=), sigues haciendo operaciones, la calculadora te calculará el resultado de esa operación primera
        if (UltimoCaracterEsOperador())
        {
            entradaActual = entradaActual.Remove(entradaActual.Length - 1);
        }
        try
        {   //Saca del resultado que operación hay que hacer con que números dada la operación en string
            string resultado = ParsearResultado(entradaActual);

            entradaActual = resultado;
            //Actualizamos la pantalla de la calculadora con el resultado obtenido
            ActualizarTextoDisplay();
        }
        catch (Exception e)
        {
            Debug.LogError("Error al calcular el resultado: " + e.Message);
        }
    }

    //Actualiza la expresión que se le muestra por pantalla al usuario con su nuevo input
    private void ActualizarTextoDisplay()
    {
        //Aquí comprobamos si ha usado el número de digitos máximos que se pueden mostrar por pantalla
        if (entradaActual.Length > MAX_CARACTERES_MOSTRADOS)
        {
            //Si se ha llegado al límite, no se mostrarán todos los carácteres, solo hasta el valor que contiene MAX_CARACTERES_MOSTRADOS
            textoDisplay.text = entradaActual.Substring(entradaActual.Length - MAX_CARACTERES_MOSTRADOS);
        }
        else
        {
            textoDisplay.text = entradaActual;
        }
    }


    //Revisa si el último carácter introducido por el usuario en la calculadora es un operador o no, en caso afirmativo devolverá true
    private bool UltimoCaracterEsOperador()
    {   //mira si el conjunto de operadores definido arriba, contiene el último carácter de la expresión
        return operadores.Contains(entradaActual[entradaActual.Length - 1]);
    }

    //Saca del resultado que operación hay que hacer con que números dada la operación en string
    private string ParsearResultado(string entradaAParsear)
    {
        //Futuro resultado final de la operación
        string resultadoParseado;
        /*
            Se usa una función de System.Data.Datatable llamada Compute, que dado una expresón y unos filtros (ignorados), te calcula un resultado que pasamos
            a que sea un double
        */
        double resultado = Convert.ToDouble(new System.Data.DataTable().Compute(entradaAParsear, ""));

        /*
            El resultado solo tendrá 5 dígitos: ("G5", CultureInfo.InvariantCulture)
            y además se mira si es tan grande que ha necesitado notación científica: Contains("E+")
            con lo que necesitaría diferente parseado
        */
        if (resultado.ToString("G5", CultureInfo.InvariantCulture).Contains("E+"))
        {
            resultadoParseado = resultado.ToString("G5", CultureInfo.InvariantCulture);
        }
        else
        {
            resultadoParseado = resultado.ToString("0.#####", CultureInfo.InvariantCulture);
        }
        return resultadoParseado;
    }

    // Color of the MR button will be white when the memory is empty and green otherwise.
    void Start()
    {
        if (memRecoverBtn != null)
        {
            memRecoverBtn.image.color = Color.white;
        }
    }

    void Update()
    {
        if (memRecoverBtn != null)
        {
            if (resultadoMemoria == "0")
            {
                memRecoverBtn.image.color = Color.white;
            }
            else
            {
                memRecoverBtn.image.color = Color.green;
            }
        }
    }
}
