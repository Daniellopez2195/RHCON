using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace rhcon.Models.ViewModel
{
    public class DataResultadosViewModel
    {

        public CategoriasViewModel categoriasVal { get; set; }
        public DominioViewModel dominiosVal { get; set; }
        public DimensionesViewModel dimensionesVal { get; set; }
        public CabeceraViewModel cabeceraVal { get; set; }
        public bool ternario { get; set; }
        public int total_empleados { get; set; }
        public int total_encuesta { get; set; }
        public string actividades { get; set; }
        public string nombreSelect { get; set; }
        public OpcionesViewModel total { get; set; }

        public int totalValue { get; set; }
        public string estadoFavorable { get; set; }

        public Cumplimiento035ViewModel cumplimiento { get; set; }
        public string responsable { get; set; }
        public string cedula { get; set; }
        public bool isEmpresa { get; set; }
        public List<respuestaEmpleado> comentarios { get; set; }
        public List<empleado> empleados { get; set; }
        public int valueTotal(SqlDataReader data, int empleados)
        {
            int valor = 0;
            while (data.Read())
            {
                valor = int.Parse(data[0].ToString());

            }

            return valor = valor / empleados;
        }
        public OpcionesViewModel Total035(SqlDataReader data, int empleados)
        {
            OpcionesViewModel total = new OpcionesViewModel();
            double valor = 0;
            while (data.Read())
            {
                valor = int.Parse(data[0].ToString());

            }

            valor = valor / empleados;

            double[] limites = { 50, 75, 99, 140 };
            string[] cadenas =
                {
                "Los resultados de la encuesta muestran un Entorno Organizacional altamente favorable y un  nivel  nulo de Riesgo Psicosocial en el centro de  trabajo;   por lo que no se requieren medidas adicionales, siempre que se mantengan las acciones que se realizan actualmente.",
                "Los resultados de la encuesta muestran un Entorno Organizacional favorable y un  nivel bajo de Riesgo Psicosocial  en el centro de  trabajo; no obstante se considera necesario una mayor difusión de la política de prevención de riesgos psicosociales y programas para: la prevención de los factores de riesgo psicosocial, la promoción de un entorno organizacional favorable y la prevención de la violencia laboral. ",
                "Los resultados de la encuesta muestran un Entorno Organizacional moderadamente favorable y un  nivel  medio de Riesgo Psicosocial  en el centro de  trabajo; por lo que se requiere revisar la política de prevención de riesgos psicosociales y programas para la prevención de los mismos, realizar promoción de un entorno organizacional favorable y la prevención de la violencia laboral, así como reforzar su aplicación y difusión, mediante un programa de intervención. ",
                "Los resultados de la encuesta muestran un Entorno Organizacional poco favorable y un  nivel  alto de Riesgo Psicosocial en el centro de  trabajo. Se requiere realizar un análisis de cada categoría y dominio, de manera que se puedan determinar las acciones de intervención apropiadas a través de un programa específico para cada una  de estas según corresponda, que podrá incluir  evaluación específica y deberá contar con  una campaña de sensibilización,  se debe revisar la política de prevención de riesgos psicosociales y los  programas para la prevención de los factores de riesgo psicosocial en el trabajo, es necesario  la promoción de un entorno organizacional favorable y tomar acciones para  prevenir y eliminar  la violencia laboral, así como reforzar las aplicaciones  y difusión de las mismas.",
                "Los resultados de la encuesta muestran un Entorno Organizacional muy desfavorable y un  nivel  muy Alto de Riesgo Psicosocial en el centro de  trabajo.   Se requiere realizar un análisis de cada categoría y dominio, de manera que se puedan determinar las acciones de intervención apropiadas a través de un programa específico para cada una según corresponda, que podrá incluir una evaluación específica y deberá incluir una campaña de sensibilización,  se debe revisar la política de prevención de riesgos psicosociales y programas para la prevención de los factores de riesgo psicosocial, es necesario  la promoción de un entorno organizacional favorable y tomar acciones para  la prevenición y eliminar  la violencia laboral, así como reforzar las aplicaciones y difusión de las mismas."
                };
            total = condicionales(valor, limites, cadenas, "");
            return total;
        }

        public CategoriasViewModel categorias(SqlDataReader data, int empleados)
        {
            CategoriasViewModel cv = new CategoriasViewModel();
            using (data)
            {
                while (data.Read())
                {
                    string cat = data[1].ToString();
                    double valor = double.Parse(data[0].ToString()) / empleados;
                    // Ambiente de trabajo

                    if (cat.Equals("Ambiente de trabajo"))

                    {
                        double[] limites = { 5, 9, 11, 14 };
                        string[] cadenas = { "", "", "", "", "" };
                        cv.ambiente_trabajo = condicionales(valor, limites, cadenas, cat);
                    }

                    //factores propios de la actividad 
                    else if (cat.Equals("Factores propios de la actividad"))
                    {
                        double[] limites = { 15, 30, 45, 60 };
                        string[] cadenas = { "", "", "", "", "" };
                        cv.factores_actividad = condicionales(valor, limites, cadenas, cat);
                    }

                    //Organizacion del tiempo de trabajo
                    else if (cat.Equals("Organización del tiempo de trabajo"))
                    {
                        double[] limites = { 5, 7, 10, 13 };
                        string[] cadenas = { "", "", "", "", "" };
                        cv.organizacion_trabajo = condicionales(valor, limites, cadenas, cat);
                    }


                    //Liderazgo y relaciones en el trabajo
                    else if (cat.Equals("Liderazgo y relaciones en el trabajo"))
                    {
                        double[] limites = { 14, 29, 42, 58 };
                        string[] cadenas = { "", "", "", "", "" };
                        cv.liderazgo_trabajo = condicionales(valor, limites, cadenas, cat);
                    }
                    //Entorno organizacional
                    else if (cat.Equals("Entorno organizacional"))
                    {
                        double[] limites = { 10, 14, 18, 23 };
                        string[] cadenas = { "", "", "", "", "" };
                        cv.entorno_organizacional = condicionales(valor, limites, cadenas, cat);
                    }
                }
            }
            return cv;
        }


        public DominioViewModel dominios(SqlDataReader data, int empleados)
        {

            DominioViewModel cv = new DominioViewModel();
            using (data)
            {
                while (data.Read())
                {
                    string cat = data[1].ToString();
                    double valor = double.Parse(data[0].ToString()) / empleados;
                    // Condiciones en el ambiente de trabajo

                    if (cat.Equals("Condiciones en el ambiente de trabajo"))

                    {
                        double[] limites = { 5, 9, 11, 14 };
                        string[] cadenas =
                        {   "La encuesta muestra que las  personas perciben la categoría de  condiciones en el ambiente de trabajo muy adecuadas.",
                            "La encuesta muestra que las  personas  perciben la categoría de  condiciones en el ambiente de trabajo adecuadas.",
                            "La encuesta muestra que las  personas  perciben la categoría de  condiciones en el ambiente de trabajo medianamente adecuadas.",
                            "La encuesta muestra que las  personas  perciben la categoría de  condiciones en el ambiente de trabajo deficientes.",
                            "La encuesta muestra que las  personas perciben la categoría de  condiciones en el ambiente de trabajo muy deficientes."
                        };
                        cv.condiciones_ambiente_trabajo = condicionales(valor, limites, cadenas);
                    }

                    //Carga de trabajo
                    else if (cat.Equals("Carga de trabajo"))
                    {
                        double[] limites = { 15, 21, 27, 37 };
                        string[] cadenas =
                            {
                            "Referente a la categoría de carga de trabajo, el personal percibe  fuerte eficiencia en la asignación de la misma.",
                            "Referente a la categoría de carga de trabajo, el personal percibe  eficiencia en la asignación de la misma.",
                            "Referente a la categoría de carga de trabajo, el personal percibe  poca eficiencia en la asignación de la misma.",
                            "Referente a la categoría de carga de trabajo, el personal percibe  deficiencia en la asignación de las misma.",
                            "Referente a la categoría de carga de trabajo, el personal percibe  en la asignación de las mismas."
                            };
                        cv.carga_trabajo = condicionales(valor, limites, cadenas);
                    }

                    //Falta de control sobre el trabajo
                    else if (cat.Equals("Falta de control sobre el trabajo"))
                    {
                        double[] limites = { 11, 16, 21, 25 };
                        string[] cadenas =
                            {
                              "El personal  indica que se siente altamente autónomo y libre referente al  control que puede tener sobre el trabajo que desempeña.",
                              "El personal  indica que se siente  autónomo y libre referente al  control que puede tener sobre el trabajo que desempeña. ",
                              "El personal  indica que se siente  poco autónomo y un tanto limitado referente al  control que puede tener sobre el trabajo que desempeña.",
                              "El personal  indica que se siente  sin autonomía  y  sin libertad referente al  control que puede tener sobre el trabajo que desempeña.",
                              "El personal  indica que se siente completamente limitado referente al  control que puede tener sobre el trabajo que desempeña. "
                            };
                        cv.falta_control_sobre_trabajo = condicionales(valor, limites, cadenas);
                    }


                    //Jornada de trabajo
                    else if (cat.Equals("Jornada de trabajo"))
                    {
                        double[] limites = { 1, 2, 4, 6 };
                        string[] cadenas = { "", "", "", "", "" };
                        cv.jornada_de_trabajo = condicionales(valor, limites, cadenas);
                    }
                    //Interferencia en la relación trabajo-familia
                    else if (cat.Equals("Interferencia en la relación trabajo-familia"))
                    {
                        double[] limites = { 4, 6, 8, 10 };
                        string[] cadenas = { "", "", "", "", "" };
                        cv.interferencia_relacion_trabajo_familia = condicionales(valor, limites, cadenas);
                    }
                    //Liderazgo
                    else if (cat.Equals("Liderazgo"))
                    {
                        double[] limites = { 9, 12, 16, 20 };
                        string[] cadenas = { "", "", "", "", "" };
                        cv.liderazgo = condicionales(valor, limites, cadenas);
                    }
                    //Liderazgo
                    else if (cat.Equals("Relaciones en el trabajo"))
                    {
                        double[] limites = { 10, 13, 17, 21 };
                        string[] cadenas = { "", "", "", "", "" };
                        cv.relaciones_trabajo = condicionales(valor, limites, cadenas);
                    }
                    //Violencia
                    else if (cat.Equals("Violencia"))
                    {
                        double[] limites = { 7, 10, 13, 16 };
                        string[] cadenas = { "", "", "", "", "" };
                        cv.violencia = condicionales(valor, limites, cadenas);
                    }
                    //Reconocimiento del desempeño
                    else if (cat.Equals("Reconocimiento del desempeño"))
                    {
                        double[] limites = { 6, 10, 14, 18 };
                        string[] cadenas = { "", "", "", "", "" };
                        cv.reconocimiento_desempeno = condicionales(valor, limites, cadenas);
                    }
                    //Insuficiente sentido de pertenencia e, inestabilidad
                    else if (cat.Equals("Insuficiente sentido de pertenencia e, inestabilidad"))
                    {
                        double[] limites = { 4, 6, 8, 10 };
                        string[] cadenas = { "", "", "", "", "" };
                        cv.insuficiente_sentido_pertenencia_inestabilidad = condicionales(valor, limites, cadenas);
                    }
                }
            }
            return cv;
        }



        public DimensionesViewModel Dimensiones(SqlDataReader data, int empleados)
        {

            DimensionesViewModel cv = new DimensionesViewModel();
            rhconEntities db = new rhconEntities();
            using (data)
            {
                while (data.Read())
                {
                    string cat = data[1].ToString();
                    double valor = double.Parse(data[0].ToString()) / empleados;
                    int idDimension = int.Parse(data[2].ToString());


                    // Condiciones peligrosas e inseguras
                    if (cat.Equals("Condiciones peligrosas e inseguras"))

                    {
                        double[] limites = { 2, 3.6, 4.4, 5.6 };
                        string[] cadenas =
{
                            "Las personas reconocen que  las actividades que desempeñan no requieren el uso de esfuerzo físico  mayor, ya que conocen  y aplican las normas de seguridad  y salud.",
                            "Las personas reconocen que  las actividades que desempeñan no requieren el uso de esfuerzo físico, ya que conocen  y aplican las normas de seguridad  y salud;",
                            "Las personas reconocen que  las actividades que desempeñan en ocasiones requieren el uso de esfuerzo físico,  que  parcialmente conocen  y  aplican las normas de seguridad  y salud;",
                            "Las personas reconocen que  las actividades que desempeñan  requieren el uso de esfuerzo físico,  dicen desconocer  las normas de seguridad  y salud;",
                            "Las personas reconocen que  las actividades que desempeñan  requieren el uso continuo de un muy alto esfuerzo físico,  que  desconocen  completamente, no aplican  normas de seguridad  y salud;"
                            };
                        cv.condiciones_peligrosas_inseguras = condicionales(valor, limites, cadenas);

                        cv.condiciones_peligrosas_inseguras.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();

                    }

                    //condiciones deficientes e insalubres
                    else if (cat.Equals("Condiciones deficientes e insalubres"))
                    {
                        double[] limites = { 2, 3.6, 4.4, 5.6 };
                        string[] cadenas =
{                           "En específico en los dominios, observan el centro de trabajo como  un lugar muy seguro con condiciones excelentes de higiene.",
                            "En específico en los dominios, observan el centro de trabajo como  un lugar seguro con buenas condiciones  de higiene.",
                            "En específico en los dominios, observan el centro de trabajo como  un lugar seguro con  condiciones  de higiene promedio.",
                            "En específico en los dominios,  observan el centro de trabajo como  un lugar inseguro con  condiciones  insalubres.",
                            "En específico en los dominios,  observan el centro de trabajo como  un lugar muy inseguro con  condiciones  insalubres."
                        };
                        cv.condiciones_deficientes_insalubres = condicionales(valor, limites, cadenas);
                        cv.condiciones_deficientes_insalubres.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    //Trabajos peligrosos
                    else if (cat.Equals("Trabajos peligrosos"))
                    {
                        double[] limites = { 1, 1.8, 2.2, 2.8 };
                        string[] cadenas =
{
                              "Consideran que las actividades que realizan son  muy seguras, por lo que se sienten sumamente tranquilos al  desempeñar las actividades diarias.",
                              "consideran que las actividades que realizan son seguras, por lo que se sienten  tranquilos al  desempeñar las actividades diarias.",
                              "consideran que las actividades que realizan son ocasionalmente poco seguras, por lo que en momentos se sienten  intranquilos al  desempeñar las actividades diarias.",
                              "consideran que las actividades que realizan son inseguras, por lo que  se sienten intranquilos al  desempeñar las actividades diarias.",
                              "consideran que las actividades que realizan son muy inseguras, por lo que  se sienten  muy intranquilos al  desempeñar las actividades diarias."
                            };

                        cv.trabajos_peligrosos = condicionales(valor, limites, cadenas);
                        cv.trabajos_peligrosos.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }


                    // Cargas cuantitativas
                    if (cat.Equals("Cargas cuantitativas"))

                    {
                        double[] limites = { 2, 2.8, 3.6, 4.9 };
                        string[] cadenas =
                        {   "El personal indica que los tiempos son respetados  y que mantiene control sobre su horario de trabajo.",
                            "El personal indica que los tiempos son respetados  y que generalmente mantiene control sobre su horario de trabajo.",
                            "El personal indica que los tiempos son  poco respetados, por lo que seguido  no  tiene control sobre su horario de trabajo.",
                            "El personal indica que los tiempos son  poco respetados, por lo que casi nunca   tiene control sobre su horario de trabajo.",
                            "El personal indica que los tiempos nunca son respetados, por lo que no  tiene control sobre su horario de trabajo."
                        };
                        cv.cargas_cuantitativas = condicionales(valor, limites, cadenas);
                        cv.cargas_cuantitativas.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Ritmos de trabajo acelerado
                    if (cat.Equals("Ritmos de trabajo acelerado"))

                    {
                        double[] limites = { 2, 2.8, 3.6, 4.9 };
                        string[] cadenas =
                        {   "que la cantidad de actividades son  adecuadas  por lo que pueden mantener un  muy buen ritmo en el desempeño diario.",
                            "que la cantidad de actividades son  adecuadas  por lo que pueden mantener un  buen ritmo normal en el desempeño diario.",
                            "que la cantidad de actividades en ocasiones les sobrepasan,  por lo que deben  mantener un  ritmo  medianamente acelerado en el desempeño diario.",
                            "que la cantidad de actividades generalmente  les sobrepasa,  por lo que deben mantener un ritmo acelerado en el desempeño diario.",
                            "que la cantidad de actividades  les sobrepasa,  por lo que debe  mantener un ritmo muy acelerado en el desempeño diario."
                        };
                        cv.ritmos_trabajo_acelerado = condicionales(valor, limites, cadenas);
                        cv.ritmos_trabajo_acelerado.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }


                    // Carga mental
                    if (cat.Equals("Carga mental"))

                    {
                        double[] limites = { 3, 4.2, 5.4, 7.4 };
                        string[] cadenas =
                        {   "que  su trabajo nunca  exige  una carga mental por arriba de la capacidad personal.",
                            "que  su trabajo casi nunca exige  una carga mental por arriba de la capacidad personal.",
                            "que  su trabajo ocasionalmente  exige  una carga mental por arriba de la capacidad personal.",
                            "que  su trabajo le exige  una carga mental por arriba de la capacidad personal.",
                            "que  su trabajo le  exige  una carga mental muy por arriba de la capacidad personal."
                        };
                        cv.carga_mental = condicionales(valor, limites, cadenas);
                        cv.carga_mental.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Cargas psicológicas emocionales
                    if (cat.Equals("Cargas psicológicas emocionales"))

                    {
                        double[] limites = { 4, 5.6, 7.2, 9.8 };
                        string[] cadenas =
                        {   "que las personas con las que se relacionan por motivo de su trabajo siempre son amables y se sienten emocionalmente muy a gusto al tratar con ellos.",
                            "que las personas con las que se relacionan por motivo de su trabajo  generalmente son amables y se sienten  emocionalmente a gusto al tratar con ellos.",
                            "que las personas con las que se relaciona por motivo de su trabajo  a veces pueden estar molestos, por lo que   esporádicamente se sienten emocionalmente  a disgusto  al tratar con ellos.",
                            "que las personas con las que se relaciona por motivo de su trabajo  casi siempre están enojados, por lo que generalmente se  siente emocionalmente  a disgusto  al tratar con ellos.",
                            "que las personas con las que se relaciona por motivo de su trabajo  siempre están enojados, por lo que todo el tiempo se  siente emocionalmente  a disgusto  al tratar con ellos."
                        };
                        cv.cargas_psicologicas_emocionales = condicionales(valor, limites, cadenas);
                        cv.cargas_psicologicas_emocionales.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Condiciones peligrosas e inseguras
                    if (cat.Equals("Cargas de alta responsabilidad"))

                    {
                        double[] limites = { 2, 2.8, 3.6, 4.9 };
                        string[] cadenas =
                        {   "En cuanto a  la responsabilidad conferida, consideran que  es congruente con las capacidades propias.",
                            "En cuanto a  la responsabilidad conferida, consideran  es congruente con las capacidades propias, aunque en esporádicas ocasiones pueden llegar a sentir que las responsabilidades los sobrepasan.",
                            "En cuanto a  la responsabilidad conferida,  a veces consideran tener una exigencia mayor a las capacidades propias y ocasionalmente se sienten sobrepasados.",
                            "En cuanto a  la responsabilidad conferida,  comúnmente consideran  tener una exigencia mayor a las capacidades propias, por lo que  se sienten sobrepasado.",
                            "En cuanto a  la responsabilidad conferida,  considera que  tiene una exigencia mayor a las capacidades propias, por lo que  se siente siempre estresado en todo momento."
                        };
                        cv.cargas_alta_responsabilidad = condicionales(valor, limites, cadenas);
                        cv.cargas_alta_responsabilidad.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Cargas contradictorias o inconsistentes
                    if (cat.Equals("Cargas contradictorias o inconsistentes"))

                    {
                        double[] limites = { 2, 2.8, 3.6, 4.9 };
                        string[] cadenas =
                        {   "además que sienten que las instrucciones que recibe en su trabajo siempre son claras,  por lo que consideran que lo que hacen es muy útil para la empresa.",
                            "además sienten que las instrucciones que reciben en su trabajo son claras,  por lo que consideran que lo que hacen es  útil para la empresa.",
                            "además que siente que las instrucciones que recibe en su trabajo en momentos  son confusas,  por lo que en algunos momentos piensan que hacen actividades innecesarias o de poco valor para la empresa.",
                            "además que siente que las instrucciones que recibe en su trabajo en momentos  son confusas,  por lo que en algunos momentos piensa que hace actividades innecesarias o de poco valor para la empresa.",
                            "además que siente que las instrucciones que recibe en su trabajo continuamente   son confusas,  por lo que en todo momento piensa que hace actividades innecesarias o de poco valor para la empresa."
                        };
                        cv.cargas_contradictorias_inconsistentes = condicionales(valor, limites, cadenas);
                        cv.cargas_contradictorias_inconsistentes.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Falta de control y autonomía sobre el trabajo
                    if (cat.Equals("Falta de control y autonomía sobre el trabajo"))

                    {
                        double[] limites = { 4.4, 6.4, 8.4, 10 };
                        string[] cadenas =
                        {   "Expone que ha sido totalmente empoderado para  adecuar las actividades a su propio ritmo y orden, tomar pausas cuando lo necesite, así mismo que  puede decidir sobre cuanto trabajo realiza durante la jornada laboral.",
                            "Expone que ha sido empoderado para  adecuar las actividades a su propio ritmo y orden, tomar pausas cuando lo necesite, así mismo que  puede decidir sobre cuanto trabajo realiza durante la jornada laboral.",
                            "Expone que ha sido  parcialmente empoderado para  adecuar las actividades a su propio ritmo y orden, tomar pausas cuando lo necesita, así mismo que poco puede decidir sobre cuanto trabajo realiza durante la jornada laboral.",
                            "Expone que casi nunca puede  adecuar las actividades a su propio ritmo y orden,  que difícilmente puede tomar pausas cuando lo necesita, así mismo que casi no tienen poder de  decidir sobre cuanto trabajo realiza durante la jornada laboral.",
                            "Expone que nunca puede  adecuar las actividades a su propio ritmo y orden,  que no  puede tomar pausas cuando lo necesita, así mismo que  no tienen poder de  decidir sobre cuanto trabajo realiza durante la jornada laboral."
                        };
                        cv.falta_control_autonomia_sobre_trabajo = condicionales(valor, limites, cadenas);
                        cv.falta_control_autonomia_sobre_trabajo.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Limitada o nula posibilidad de desarrollo
                    if (cat.Equals("Limitada o nula posibilidad de desarrollo"))

                    {
                        double[] limites = { 2.2, 3.2, 4.2, 5 };
                        string[] cadenas =
                        {   "Siente que en su trabajo tiene la oportunidad de desarrollar nuevas habilidades y siempre tiene la oportunidad de aspirar a  crecimiento profesional para ocupar un puesto de mayor jerarquía.",
                            "Siente que en su trabajo tiene la oportunidad de desarrollar nuevas habilidades y casi siempre tiene la oportunidad de aspirar a crecimiento profesional para ocupar un puesto de mayor jerarquía.",
                            "Siente que en su trabajo tiene algunas veces  la oportunidad de desarrollar nuevas habilidades y solo a veces tiene la oportunidad de aspirar a  crecimiento profesional para ocupar un puesto de mayor jerarquía.",
                            "Siente que  casi nunca en su trabajo tiene  la oportunidad de desarrollar nuevas habilidades y rara vez tiene la oportunidad de aspirar a  crecimiento profesional para ocupar un puesto de mayor jerarquía.",
                            "Siente que en su trabajo NO tiene  la oportunidad de desarrollar nuevas habilidades y NO tiene la oportunidad de aspirar a  crecimiento profesional para ocupar un puesto de mayor jerarquía."
                        };
                        cv.limitada_nula_posibilidad_desarrollo = condicionales(valor, limites, cadenas);
                        cv.limitada_nula_posibilidad_desarrollo.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Insuficiente participación y manejo del cambio
                    if (cat.Equals("Insuficiente participación y manejo del cambio"))

                    {
                        double[] limites = { 2.2, 3.2, 4.2, 5 };
                        string[] cadenas =
                        {   "Expresa que en todo momento  es tomado en cuenta para realizar cambios e implementar ideas en su trabajo y que los mismos cambios no representan dificultad alguna para el desempeño de sus labores.",
                            "Expresa que casi siempre es tomado en cuenta para realizar cambios e implementar ideas en su trabajo y que los mismos cambios no representan dificultad alguna para el desempeño de sus labores.",
                            "Expresa que en ocasiones es tomado en cuenta para realizar cambios e implementar ideas en su trabajo y que los mismos cambios ocasionalmente no representan dificultad alguna para el desempeño de sus labores.",
                            "Expresa que en ocasiones es tomado en cuenta para realizar cambios e implementar ideas en su trabajo, sien embargo que los mismos cambios ocasionalmente no representan dificultad alguna para el desempeño de sus labores.",
                            "Expresa que NO es tomado en cuenta para realizar cambios e implementar ideas en su trabajo y que los cambios son una  dificultad  para el desempeño de sus labores."
                        };
                        cv.insuficiente_participacion_manejo_cambio = condicionales(valor, limites, cadenas);
                        cv.insuficiente_participacion_manejo_cambio.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Limitada o inexistente capacitación
                    if (cat.Equals("Limitada o inexistente capacitación"))

                    {
                        double[] limites = { 2.2, 3.2, 4.2, 5 };
                        string[] cadenas =
                        {   "Manifiesta que siempre se le permite  asistir a capacitaciones relacionadas con su trabajo, al mismo tiempo reconoce que recibe por parte de la empresa capacitación útil y constante.",
                            "Manifiesta que comúnmente se le permite  asistir a capacitaciones relacionadas con su trabajo, al mismo tiempo reconoce que frecuentemente recibe por parte de la empresa capacitación útil y constante.",
                            "Manifiesta que algunas veces se le permite  asistir a capacitaciones relacionadas con su trabajo, al mismo tiempo reconoce que recibe por parte de la empresa capacitación útil, pero de manera esporádica.",
                            "Manifiesta que casi nunca se le permite  asistir a capacitaciones relacionadas con su trabajo, al mismo tiempo expresa que recibe por parte de la empresa muy poca  capacitación útil para el desempeño de su funciones.",
                            "Manifiesta que nunca se le permite  asistir a capacitaciones relacionadas con su trabajo, al mismo tiempo expresa que NO recibe  capacitación útil por parte de la empresa para el desempeño de su funciones."
                        };
                        cv.limitada_inexistente_capacitacion = condicionales(valor, limites, cadenas);
                        cv.limitada_inexistente_capacitacion.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    //Jornadas de trabajo extensas
                    if (cat.Equals("Jornadas de trabajo extensas"))

                    {
                        double[] limites = { 1, 2, 4, 6 };
                        string[] cadenas =
                        {   "Las y los trabajadores manifiestan referente a las jornadas de trabajo que son muy adecuadas, que  nunca tiene que trabajar tiempo extraordinario, que no trabajan en días de descanso festivos y/o fines de semana.",
                            "Las y los trabajadores manifiestan referente a las jornadas de trabajo que son adecuadas, que casi nunca tiene que trabajar tiempo extraordinario, que rara vez trabaja en días de descanso festivos y/o fines de semana.",
                            "Las y los trabajadores manifiestan que referente a las jornadas de trabajo son parcialmente adecuadas, que  algunas veces tiene que trabajar tiempo extraordinario e igualmente algunas veces trabaja en días de descanso festivos y/o fines de semana.",
                            "Las y los trabajadores manifiestan que referente a las jornadas de trabajo son poco adecuadas, que  la mayoría de las veces tiene que trabajar tiempo extraordinario e igualmente casi siempre tienen que trabajar en días de descanso festivos y/o fines de semana.",
                            "Las y los trabajadores manifiestan que referente a las jornadas de trabajo son exhaustivas, que todas  las semanas tienen que trabajar mas de 3 días tiempo extraordinario, por más de 3 horas cada día y/o  que siempre tienen que laborar en días de descanso festivos y fines de semana."
                        };
                        cv.jornadas_trabajo_extensas = condicionales(valor, limites, cadenas);
                        cv.jornadas_trabajo_extensas.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Influencia del trabajo fuera del centro laboral
                    if (cat.Equals("Influencia del trabajo fuera del centro laboral"))

                    {


                        double[] limites = { 2, 3, 4, 5 };
                        string[] cadenas =
                        {   "Con relación  a la categoría de balance trabajo- familia, consideran que dado que la jornada de trabajo es muy adecuada, les permite tener actividades familiares y personales y nunca tienen que atender asuntos de trabajo  cuando ya salieron de su centro de trabajo, es decir desde casa.",
                            "Con relación  a la categoría de balance trabajo- familia, consideran que dado que la jornada de trabajo es adecuada, les permite tener actividades familiares y personales y  casi nunca tienen que atender asuntos de trabajo  cuando ya salieron de su centro de trabajo, es decir desde casa.",
                            "Con relación  a la categoría de balance trabajo- familia, consideran que dado que la jornada de trabajo es  parcialmente adecuada, pocas veces les permite tener actividades familiares y personales y  algunas veces tienen que atender asuntos de trabajo  cuando ya salieron de su centro de trabajo, es decir desde casa.",
                            "Con relación  a la categoría de balance trabajo- familia, consideran que dado que la jornada de trabajo es  poco adecuada, casi nunca puede tener actividades familiares y personales y  continuamente tienen que atender asuntos de trabajo  cuando ya salieron de su centro de trabajo, es decir desde casa.",
                            "Con relación  a la categoría de balance trabajo- familia, consideran que dado que la jornada de trabajo es  deficiente,  nunca pueden tener actividades familiares y personales y  siempre tienen que atender asuntos de trabajo cuando ya salieron de su centro de trabajo, es decir desde casa."
                        };
                        cv.influencia_trabajo_fuera_centro_laboral = condicionales(valor, limites, cadenas);
                        cv.influencia_trabajo_fuera_centro_laboral.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Influencia del trabajo fuera del centro laboral
                    if (cat.Equals("Influencia de las responsabilidades familiares"))

                    {
                        double[] limites = { 2, 3, 4, 5 };
                        string[] cadenas =
                        {   "El personal se siente tranquilo en las responsabilidades familiares/ personales, por lo que no se distrae pensando  durante su jornada de trabajo en ellas y así mismo considera que no les  afectan en lo absoluto en el trabajo que desempeña.",
                            "El personal rara vez se siente  preocupado  y pensando en las responsabilidades familiares/ personales durante su jornada de trabajo; y considera que estas responsabilidades  no  afectan el trabajo que desempeña.",
                            "El personal se siente algunas veces  preocupado  y pensando en las responsabilidades familiares/ personales durante su jornada de trabajo y considera que estas responsabilidades  en gran parte afecta el trabajo que desempeña.",
                            "El personal se siente casi siempre preocupado  y pensando en las responsabilidades familiares/ personales durante su jornada de trabajo y considera que estas responsabilidades  pueden afectar el trabajo que desempeña.",
                            "El personal se siente siempre  muy preocupado  y pensando en las responsabilidades familiares/ personales durante su jornada de trabajo y considera que estas responsabilidades afectan el trabajo que desempeña."
                        };
                        cv.influencia_responsabilidades_familiares = condicionales(valor, limites, cadenas);
                        cv.influencia_responsabilidades_familiares.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    //Escaza claridad de funciones
                    if (cat.Equals("Escaza claridad de funciones"))

                    {
                        double[] limites = { 4, 5.3, 7.1, 8.8 };
                        string[] cadenas =
                        {   "Las y los colaboradores, manifiestan que el liderazgo del jefe inmediato es muy positivo; que son informados con claridad y oportunamente sobre cuáles son sus funciones, los resultados  y objetivos a lograr.",
                            "Las y los colaboradores, manifiestan que el liderazgo del jefe inmediato es  positivo; que casi siempre son informados  con claridad y oportunamente sobre cuales son sus funciones, los resultados  y objetivos a lograr.",
                            "Las y los colaboradores, manifiestan que el liderazgo del jefe inmediato es medianamente positivo; que algunas veces son informados  con claridad y oportunamente sobre cuales son sus funciones, los resultados  y objetivos a lograr.",
                            "Las y los colaboradores, manifiestan que el liderazgo del jefe inmediato es deficiente; que rara vez son informados  con claridad y oportunamente sobre cuales son sus funciones, los resultados  y objetivos a lograr.",
                            "Las y los colaboradores, manifiestan que el liderazgo del jefe inmediato es sumamente negativo; que nunca son informados  con claridad y oportunamente sobre cuales son sus funciones, los resultados  y objetivos a lograr."
                        };
                        cv.escaza_claridad_funcionales = condicionales(valor, limites, cadenas);
                        cv.escaza_claridad_funcionales.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Características del liderazgo
                    if (cat.Equals("Características del liderazgo"))

                    {
                        double[] limites = { 5, 6.6, 8.8, 11.11 };
                        string[] cadenas =
                        {   "Expresan que el jefe ayuda en la organización de las actividades, les toma en cuenta su punto de vista, les informa de manera oportuna cualquier situación sobre el trabajo, les orienta y ayuda a solucionar problemas que se presentan.",
                            "Expresan que el jefe comúnmente  ayuda en la organización de las actividades, les toma en cuenta su punto de vista, les informa de manera oportuna cualquier situación sobre el trabajo, les orienta y ayuda a solucionar problemas que se presentan.",
                            "Expresan que el jefe algunas veces les ayuda en la organización de las actividades, a veces les toma en cuenta su punto de vista, parcialmente les informa de manera oportuna cualquier situación sobre el trabajo, les orienta y ayuda a solucionar problemas que se presentan.",
                            "Expresan que el jefe casi nunca les ayuda en la organización de las actividades, poco les toma en cuenta su punto de vista, difícilmente les informa sobre las situaciones de trabajo con las que están relacionados, poco les orienta y casi nunca les ayuda a solucionar problemas que se presentan.",
                            "Expresan que el jefe  no les ayuda en la organización de las actividades, no  les toma en cuenta su punto de vista, no  les informa sobre las situaciones de trabajo con las que están relacionados, no les orienta y  nunca les ayuda a solucionar problemas que se presentan."
                        };
                        cv.caracteristicas_liderazgo = condicionales(valor, limites, cadenas);
                        cv.caracteristicas_liderazgo.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }


                    // Relaciones sociales en el trabajo
                    if (cat.Equals("Relaciones sociales en el trabajo"))

                    {
                        double[] limites = { 5.5, 7.2, 9.4, 11.6 };
                        string[] cadenas =
                        {   "En relación con los compañeros y compañeras de trabajo en general expresan que existe una confianza plena de trabajo,  que existe un respeto total, que se sienten parte de un grupo, que el trabajo en equipo siempre predomina y se ayudan entre sí cuando tienen dificultades en el desempeño de sus funciones.",
                            "En relación con los compañeros y compañeras de trabajo en general expresan que existe una confianza de trabajo y de forma respetuosa, que casi siempre se sienten parte de un grupo, que el trabajo en equipo  predomina y  se ayudan entre sí cuando tienen dificultades en el desempeño de sus funciones.",
                            "En relación con los compañeros y compañeras de trabajo expresan que a veces sienten confianza de trabajo y ocasionalmente falta respeto, que solo algunas veces se sienten parte de un grupo y  que el trabajo en equipo  es esporádico,  que poco se ayudan entre sí cuando tienen dificultades en el desempeño de sus funciones.",
                            "En relación con los compañeros y compañeras de trabajo expresan que sienten  casi nada de confianza  y comúnmente existe falta de respeto entre ellos, casi nunca se sienten parte de un grupo y  que el trabajo en equipo  es casi nulo,  que difícilmente se ayudan entre sí cuando tienen dificultades en el desempeño de sus funciones.",
                            "En relación con los compañeros y compañeras de trabajo expresan que sienten desconfianza  y  falta de respeto total  entre ellos, no se sienten parte de un grupo y  no existe trabajo en equipo, que no  se ayudan entre sí cuando tienen dificultades en el desempeño de sus funciones."
                        };
                        cv.relaciones_sociales_trabajo = condicionales(valor, limites, cadenas);
                        cv.relaciones_sociales_trabajo.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }


                    //Deficiente relación con los colaboradores que supervisa
                    if (cat.Equals("Deficiente relación con los colaboradores que supervisa"))

                    {
                        double[] limites = { 4.4, 5.7, 7.5, 9.3 };
                        string[] cadenas =
                        {   "Las y los supervisores del centro de trabajo es decir los jefes inmediatos, expresan que el personal a su cargo, les comunica en forma oportuna los asuntos de trabajo, cumplen con los resultados que se les establecen, cooperan cuando se les requiere y siempre toman en cuenta las sugerencias que ellos les dan.",
                            "Las y los supervisores del centro de trabajo es decir los jefes inmediatos, expresan que el personal a su cargo, generalmente les comunica en forma oportuna los asuntos de trabajo, casi siempre cumplen con los resultados que se les establecen, cooperan cuando se les requiere y la mayoría de las veces toman en cuenta las sugerencias que ellos les dan.",
                            "Las y los supervisores del centro de trabajo es decir los jefes inmediatos, expresan que el personal a su cargo, poco  les comunica en forma oportuna los asuntos de trabajo, únicamente algunas veces cumplen con los resultados que se les establecen, poco cooperan cuando se les requiere y solo algunas veces toman en cuenta las sugerencias que ellos les dan. ",
                            "Las y los supervisores del centro de trabajo es decir los jefes inmediatos, expresan que el personal a su cargo, muy rara vez  les comunica en forma oportuna los asuntos de trabajo, casi nunca cumplen con los resultados que se les establecen, poco cooperan cuando se les requiere y difícilmente toman en cuenta las sugerencias que ellos les dan.",
                            "Las y los supervisores del centro de trabajo es decir los jefes inmediatos, expresan que el personal a su cargo, nunca  les comunica en forma oportuna los asuntos de trabajo, que no cumplen con los resultados que se les establecen, no cooperan cuando se les requiere,  no les hacen caso y no  toman en cuenta las sugerencias que ellos les dan. "
                        };
                        cv.deficiente_relacion_colaboradores_supervisa = condicionales(valor, limites, cadenas);
                        cv.deficiente_relacion_colaboradores_supervisa.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }


                    // Violencia laboral
                    if (cat.Equals("Violencia laboral"))

                    {
                        double[] limites = { 7, 10, 13, 16 };
                        string[] cadenas =
                        {   "Las personas en el centro de trabajo sienten que siempre pueden expresar su punto de vista sin interrupciones, que nunca han sido criticadas de manera personal, que existe un respeto absoluto y que nunca se burlan de el o ella; que siempre se les  toma en cuenta tanto en las reuniones de trabajo como en la toma de decisiones; que siempre se siente reconocidos sobre sus  éxitos laborales, reconocen que tienen oportunidades para mejorar en el trabajo y que nunca ha presenciado actos de violencia laboral como hostigamiento, acoso o malos tratos en contra de ellos mismos u otra persona.",
                            "Las personas en el centro de trabajo sienten que casi siempre pueden expresar su punto de vista sin interrupciones, que alguna vez  han sido criticada de manera personal, pero que en general existe respeto; que  usualmente  se les  toma en cuenta tanto en las reuniones de trabajo como en la toma de decisiones; que continuamente se siente reconocido sobre sus  éxitos laborales;  reconocen que comúnmente se les otorga  oportunidades para mejorar en el trabajo y que rara vez  han presenciado actos de violencia laboral como hostigamiento,  burlas, acoso o malos tratos en contra de ellos mismos u otra persona.",
                            "Las personas en el centro de trabajo sienten que a veces  pueden expresar su punto de vista sin interrupciones, que algunas veces han sido criticadas de manera personal, que existe poco  respeto; que  rara vez  se les  toma en cuenta tanto en las reuniones de trabajo como en la toma de decisiones; que a veces se siente poco reconocido sobre sus  éxitos laborales;  manifiestan que se les otorga de manera esporádica  oportunidades para mejorar en el trabajo e inclusive  han presenciado actos de violencia laboral como hostigamiento,  burlas, acoso o malos tratos en contra de ellos mismos u otra persona.",
                            "Las personas en el centro de trabajo sienten que casi nunca se les permite  expresar su punto, que  muchas veces  han sido criticadas de manera personal, que existe muy poco  respeto; que  casi nunca se les  toma en cuenta tanto en las reuniones de trabajo como en la toma de decisiones; que  se siente poco reconocido sobre sus  éxitos laborales;  manifiestan que son casi nulas las  oportunidades para mejorar en el trabajo y tomar puestos de mayor jerarquía; y que comúnmente existen actos de violencia laboral como hostigamiento,  burlas, acoso o malos tratos en contra de ellos mismos u otra persona.",
                            "Las personas en el centro de trabajo sienten que no se les permite  expresar su punto de vista, que siempre son  criticadas de manera personal, que no existe  respeto; que nunca se les  toma en cuenta tanto para las reuniones de trabajo como en la toma de decisiones; que  se siente no reconocidos sobre sus  éxitos laborales;  manifiestan que son  nulas las  oportunidades para mejorar en el trabajo y tomar puestos de mayor jerarquía; y que todo el tiempo existen actos de violencia laboral como hostigamiento,  burlas, acoso o malos tratos en contra de ellos mismos u otra persona."
                        };
                        cv.violencia_laboral = condicionales(valor, limites, cadenas);
                        cv.violencia_laboral.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Escasa o nula retroalimentación del desempeño
                    if (cat.Equals("Escasa o nula retroalimentación del desempeño"))

                    {
                        double[] limites = { 2, 3.3, 4.6, 6 };
                        string[] cadenas =
                        {   "Las y los colaboradores reconocen que reciben retroalimentación sobre lo que hacen bien en el trabajo y que su desempeño es evaluado positivamente, lo que les ayuda a mejorar.",
                            "Las y los colaboradores reconocen que continuamente reciben retroalimentación sobre lo que hacen bien en el trabajo y que su desempeño es evaluado positivamente, lo que casi siempre les ayuda a mejorar.",
                            "Las y los colaboradores reconocen que ocasionalmente reciben retroalimentación sobre lo que hacen bien en el trabajo y que su desempeño es evaluado parcialmente, lo que algunas veces les ayuda a mejorar.",
                            "Las y los colaboradores reconocen que rara vez reciben retroalimentación sobre lo que hacen bien en el trabajo y que su desempeño casi nunca es evaluado, por lo que poco les ayuda a mejorar.",
                            "Las y los colaboradores reconocen que nunca reciben retroalimentación sobre lo que hacen bien en el trabajo y que su desempeño no es evaluado, por lo que no reciben  ayuda para mejorar."
                        };
                        cv.escasa_nula_retroalimentacion_desempeno = condicionales(valor, limites, cadenas);
                        cv.escasa_nula_retroalimentacion_desempeno.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Escaso o nulo reconocimiento y compensación
                    if (cat.Equals("Escaso o nulo reconocimiento y compensación"))

                    {
                        double[] limites = { 4, 6.6, 9.3, 12 };
                        string[] cadenas =
                        {   "Que el pago del salario es siempre puntual y se sienten reconocidos y recompensados justamente; igualmente perciben que las personas que hacen bien su trabajo pueden crecer laboralmente.",
                            "Que el pago de el salario casi siempre es puntual y que se sienten  reconocidos y recompensados justamente; igualmente perciben que las personas que hacen bien su trabajo pueden crecer laboralmente.",
                            "Que el pago del salario comúnmente es puntual y que solo algunas veces se sienten parcialmente reconocidos y recompensados justamente; igualmente perciben que algunas veces las personas que hacen bien su trabajo pueden crecer laboralmente.",
                            "Que el pago del salario rara vez es puntual y que casi nunca se sienten  reconocidos y recompensados justamente; igualmente perciben que son muy pocas las personas que hacen bien su trabajo y que se les otorga oportunidad de crecer laboralmente.",
                            "Que el pago del salario nunca es puntual y que no se sienten  reconocidos y que la compensación que reciben es injusta porque trabajan más de lo que se les paga; igualmente perciben que no existen oportunidades  de crecimiento laboral  para las personas que hacen bien su trabajo."
                        };
                        cv.escasa_nulo_reconocimiento_compensacion = condicionales(valor, limites, cadenas);
                        cv.escasa_nulo_reconocimiento_compensacion.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Limitado sentido de pertenencia
                    if (cat.Equals("Limitado sentido de pertenencia"))

                    {
                        double[] limites = { 2, 3, 4, 5 };
                        string[] cadenas =
                        {   "Los empleados expresan que se sienten muy orgullosos de pertenecer a la empresa y están muy comprometidos con su trabajo.",
                            "Los empleados expresan que se sienten orgullosos de pertenecer a la empresa y están comprometidos con su trabajo.",
                            "Los empleados expresan que a veces se sienten orgullosos de pertenecer a la empresa y parcialmente comprometidos con su trabajo.",
                            "Los empleados expresan que  se sienten poco orgullosos de pertenecer a la empresa y con un bajo compromiso con su trabajo.",
                            "Los empleados expresan que no están orgullosos de pertenecer a la empresa y que no tienen compromiso con su trabajo."
                        };
                        cv.limitado_sentido_pertencia = condicionales(valor, limites, cadenas);
                        cv.limitado_sentido_pertencia.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                    // Inestabilidad laboral
                    if (cat.Equals("Inestabilidad laboral"))

                    {
                        double[] limites = { 2, 3, 4, 5 };
                        string[] cadenas =
                        {   "Perciben que el trabajo es muy  estable,  y que no existe rotación de personal.",
                            "Perciben que el trabajo es estable,  y que casi no existe rotación de personal.",
                            "Perciben que su trabajo es medianamente  estable,  y que existe algo de rotación de personal.",
                            "Perciben que su trabajo es muy inestable  y que la gente continuamente se va de la empresa.",
                            "Perciben que su trabajo es muy  inestable, que la gente todo el tiempo se va de la empresa en periodos muy cortos."
                        };
                        cv.inestabilidad_laboral = condicionales(valor, limites, cadenas);
                        cv.inestabilidad_laboral.prevenciones = db.prevenciones.Where(d => d.idDimencion == idDimension).ToList();
                    }

                }
            }
            return cv;
        }


        public Cumplimiento035ViewModel cumplimiento035(int total_empleados, int total_encuenta, bool ternario, string empresa, string centro)
        {
            Cumplimiento035ViewModel data = new Cumplimiento035ViewModel();
            string rango = "empresa";


            if (total_empleados <= 15)
            {
                data.valor = 0;
                data.ternanrio = true;
                if (ternario)
                {
                    data.mensaje = "La empresa " + empresa + " en este centro de trabajo no está obligada a realizar la Identificación y " +
                    "análisis de los factores de riesgo psicosocial y la evaluación de entorno organizacional, " +
                    "dado que cuenta con 15 o menos personas laborando en el mismo..";
                }
                else
                {
                    data.mensaje = "La empresa " + empresa + " no está obligada a realizar la Identificación y " +
                    "análisis de los factores de riesgo psicosocial y la evaluación de entorno organizacional, " +
                    "dado que cuenta con 15 o menos personas laborando en el mismo..";
                }
            }
            else if (total_empleados <= 50)
            {
                data.valor = total_empleados;
                if (total_encuenta == total_empleados)
                {
                    data.ternanrio = true;

                }
                else
                {
                    data.ternanrio = false;
                    if (ternario)
                    {
                        data.mensaje = "La empresa " + empresa + " en este centro de trabajo está obligada a realizar la Identificación" +
                            " y análisis de los factores de riesgo psicosocial, dado que cuenta con 16 a 50 personas laborando en el mismo.";
                    }
                    else
                    {
                        data.mensaje = "La empresa " + empresa + " está obligada a realizar la Identificación" +
                            " y análisis de los factores de riesgo psicosocial, dado que cuenta con 16 a 50 personas laborando en el mismo.";
                    }
                }

            }
            else if (total_empleados < 50)
            {

                double muestra_G = Math.Round((0.9604 * total_empleados) / (0.0025 * (total_empleados - 1) + 0.9604));
                int muestra = Convert.ToInt32(muestra_G);
                data.valor = muestra;
                if (total_encuenta >= muestra)
                {
                    data.ternanrio = true;

                }
                else
                {
                    data.ternanrio = false;
                    if (ternario)
                    {
                        data.mensaje = "La empresa " + empresa + " en este centro de trabajo está obligada a realizar la Identificación" +
                            " y análisis de los factores de riesgo psicosocial y la evaluación de entorno organizacional," +
                            " dado que cuenta con 51 o más personas laborando en el mismo.";
                    }
                    else
                    {
                        data.mensaje = "La empresa" + empresa + " está obligada a realizar la Identificación" +
                            " y análisis de los factores de riesgo psicosocial y la evaluación de entorno organizacional," +
                            " dado que cuenta con 51 o más personas laborando en el mismo.";
                    }
                }

            }

            return data;
        }


        public CabeceraViewModel Cabecera(SqlDataReader data, int empleados)
        {

            CabeceraViewModel cv = new CabeceraViewModel();
            var total = 0;
            using (data)
            {
                while (data.Read())
                {
                    string cat = data[1].ToString();
                    double divicion = int.Parse(data[0].ToString()) / empleados;
                    double[] limites = { 2.5, 1.5, 1 };
                    int totalPuntos = 168;
                    total += int.Parse(data[0].ToString());
                    // Para responder las siguientes preguntas considera tu percepción sobre accesibilidad"
                    if (cat.Equals("Para responder las siguientes preguntas considera tu percepción sobre accesibilidad"))

                    {
                        using (rhconEntities db = new rhconEntities())
                        {
                            var consulta = db.EncabezadoCuestionario.Where(d => d.descripcion.Equals(cat)).First();

                            int maximo = consulta.maximo.Value;
                            double resultado = divicion / maximo;
                            string[] cadenas = consulta.opciones_text.Split('%');
                            cv.accesibilidad = condicionales025(resultado, limites, cadenas);
                        }

                    }
                    // Para responder las siguientes preguntas considera tu percepción sobre acoso y hostigamiento
                    if (cat.Equals("Para responder las siguientes preguntas considera tu percepción sobre acoso y hostigamiento"))

                    {
                        using (rhconEntities db = new rhconEntities())
                        {
                            var consulta = db.EncabezadoCuestionario.Where(d => d.descripcion.Equals(cat)).First();

                            int maximo = consulta.maximo.Value;
                            double resultado = divicion / maximo;
                            string[] cadenas = consulta.opciones_text.Split('%');
                            cv.acoso_hostigamiento = condicionales025(resultado, limites, cadenas);
                        }

                    }
                    // Para responder las siguientes preguntas considera tu percepción sobre clima laboral libre de violencia"
                    if (cat.Equals("Para responder las siguientes preguntas considera tu percepción sobre clima laboral libre de violencia"))

                    {
                        using (rhconEntities db = new rhconEntities())
                        {
                            var consulta = db.EncabezadoCuestionario.Where(d => d.descripcion.Equals(cat)).First();

                            int maximo = consulta.maximo.Value;
                            double resultado = divicion / maximo;
                            string[] cadenas = consulta.opciones_text.Split('%');
                            cv.clima_laboral_violencia = condicionales025(resultado, limites, cadenas);
                        }

                    }
                    // Para responder las siguientes preguntas considera tu percepción sobre condiciones generales de trabajo
                    if (cat.Equals("Para responder las siguientes preguntas considera tu percepción sobre condiciones generales de trabajo"))

                    {
                        using (rhconEntities db = new rhconEntities())
                        {
                            var consulta = db.EncabezadoCuestionario.Where(d => d.descripcion.Equals(cat)).First();

                            int maximo = consulta.maximo.Value;
                            double resultado = divicion / maximo;
                            string[] cadenas = consulta.opciones_text.Split('%');
                            cv.condiciones_generales_trabajo = condicionales025(resultado, limites, cadenas);
                        }

                    }
                    // Para responder las siguientes preguntas considera tu percepción sobre corresponsabilidad en la vida laboral, familiar y personal
                    if (cat.Equals("Para responder las siguientes preguntas considera tu percepción sobre corresponsabilidad en la vida laboral, familiar y personal"))

                    {
                        using (rhconEntities db = new rhconEntities())
                        {
                            var consulta = db.EncabezadoCuestionario.Where(d => d.descripcion.Equals(cat)).First();

                            int maximo = consulta.maximo.Value;
                            double resultado = divicion / maximo;
                            string[] cadenas = consulta.opciones_text.Split('%');
                            cv.corresponsabilidad_vida_laboral_familiar = condicionales025(resultado, limites, cadenas);
                        }

                    }
                    // Para responder las siguientes preguntas considera tu percepción sobre formación y capacitación
                    if (cat.Equals("Para responder las siguientes preguntas considera tu percepción sobre formación y capacitación"))

                    {
                        using (rhconEntities db = new rhconEntities())
                        {
                            var consulta = db.EncabezadoCuestionario.Where(d => d.descripcion.Equals(cat)).First();

                            int maximo = consulta.maximo.Value;
                            double resultado = divicion / maximo;
                            string[] cadenas = consulta.opciones_text.Split('%');
                            cv.formacion_capacitacion = condicionales025(resultado, limites, cadenas);
                        }

                    }
                    // Para responder las siguientes preguntas considera tu percepción sobre permanencia y ascenso
                    if (cat.Equals("Para responder las siguientes preguntas considera tu percepción sobre permanencia y ascenso"))

                    {
                        using (rhconEntities db = new rhconEntities())
                        {
                            var consulta = db.EncabezadoCuestionario.Where(d => d.descripcion.Equals(cat)).First();

                            int maximo = consulta.maximo.Value;
                            double resultado = divicion / maximo;
                            string[] cadenas = consulta.opciones_text.Split('%');
                            cv.permanencia_ascenso = condicionales025(resultado, limites, cadenas);
                        }

                    }
                    //Para responder las siguientes preguntas considera tu percepción sobre reclutamiento y selección de personal
                    if (cat.Equals("Para responder las siguientes preguntas considera tu percepción sobre reclutamiento y selección de personal"))

                    {
                        using (rhconEntities db = new rhconEntities())
                        {
                            var consulta = db.EncabezadoCuestionario.Where(d => d.descripcion.Equals(cat)).First();

                            int maximo = consulta.maximo.Value;
                            double resultado = divicion / maximo;
                            string[] cadenas = consulta.opciones_text.Split('%');
                            cv.reclutamiento_seleccion_personas = condicionales025(resultado, limites, cadenas);
                        }

                    }
                    // Para responder las siguientes preguntas considera tu percepción sobre respeto a la diversidad
                    if (cat.Equals("Para responder las siguientes preguntas considera tu percepción sobre respeto a la diversidad"))

                    {
                        using (rhconEntities db = new rhconEntities())
                        {
                            var consulta = db.EncabezadoCuestionario.Where(d => d.descripcion.Equals(cat)).First();

                            int maximo = consulta.maximo.Value;
                            double resultado = divicion / maximo;
                            string[] cadenas = consulta.opciones_text.Split('%');
                            cv.respeto_diversidad = condicionales025(resultado, limites, cadenas);
                        }

                    }


                }

            }
            // resultado general 
            double[] limites_generales = { 2.5, 1.5, 1 };
            string[] cadenas_generales = {
                "Excelente percepción del clima laboral y no discriminación del centro de trabajo con apartados de referencia que conforman el entorno laboral, que propician alta productividad en el desempeño de las y los trabajadores",
                "Muy buena percepción del clima laboral y no discriminación del centro de trabajo con apartados de referencia que conforman el entorno laboral, que propician productividad en el desempeño de las y los trabajadores, con mínimas áreas de oportunidad en algunos de los apartados de referencia.",
                "Aceptable percepción del clima laboral y no discriminación del centro de trabajo con varios apartados de referencia que conforman el entorno laboral y que muestran áreas de oportunidad por lo que en ocasiones puede dificultar el desempeño de las y los trabajadores",
                "Deficiente percepción del clima laboral y no discriminación del centro de trabajo con un número considerable de apartados de referencia que conforman el entorno laboral, que muestran áreas de oportunidad por lo que puede ser continuamente difícil el desempeño de las y los trabajadores ",
                "Muy mala percepción del clima laboral y no discriminación del centro de trabajo con gran número de apartados de referencia que conforman el entorno laboral, mostrando áreas de oportunidad por lo que puede ser muy difícil el desempeño de las y los trabajadores"
                };
            double resultado_general = total / 56;
            cv.resultado_general = condicionales025(resultado_general, limites_generales, cadenas_generales);
            return cv;
        }

        public OpcionesViewModel condicionales(double valor, double[] limites, string[] cadenas, string tipo = "")
        {
            string[] colores = { "azul", "verde", "amarillo", "naranja", "rojo" };
            string[] estado = { "Nulo", "Bajo", "Medio", "Alto", "Muy alto" };
            OpcionesViewModel opciones = new OpcionesViewModel();


            if (valor < limites[0])
            {
                opciones.text = cadenas[0];
                opciones.color = colores[0];
                opciones.estado = estado[0];
                opciones.tipo = tipo;

            }
            else if (valor < limites[1])
            {
                opciones.text = cadenas[1];
                opciones.color = colores[1];
                opciones.estado = estado[1];
                opciones.tipo = tipo;
            }
            else if (valor < limites[2])
            {
                opciones.text = cadenas[2];
                opciones.color = colores[2];
                opciones.estado = estado[2];
                opciones.tipo = tipo;
            }
            else if (valor < limites[3])
            {
                opciones.text = cadenas[3];
                opciones.color = colores[3];
                opciones.estado = estado[3];
                opciones.tipo = tipo;
            }
            else if (valor >= limites[3])
            {
                opciones.text = cadenas[4];
                opciones.color = colores[4];
                opciones.estado = estado[4];
                opciones.tipo = tipo;
            }

            return opciones;
        }

        public OpcionesViewModel condicionales025(double valor, double[] limites, string[] cadenas)
        {
            string[] colores = { "azul", "verde", "naranja", "rojo" };
            string[] estado = { "Excelente", "Bueno", "Regular", "Malo" };
            OpcionesViewModel opciones = new OpcionesViewModel();

            double exde = Math.Round(valor, 2);
            if (exde >= limites[0])
            {
                opciones.text = cadenas[0];
                opciones.color = colores[0];
                opciones.estado = estado[0];

            }
            else if (exde >= limites[1])
            {
                opciones.text = cadenas[1];
                opciones.color = colores[1];
                opciones.estado = estado[1];
            }
            else if (exde >= limites[2])
            {
                opciones.text = cadenas[2];
                opciones.color = colores[2];
                opciones.estado = estado[2];
            }
            else if (exde < limites[2])
            {
                opciones.text = cadenas[3];
                opciones.color = colores[3];
                opciones.estado = estado[3];
            }

            return opciones;
        }
    }
}