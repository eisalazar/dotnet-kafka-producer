# Publicador Integra basado en Kafka Streams

## Topologia

La topologia se basa en un stream que toma como input el topico donde se estaran publicando los registros de entityTraceability. Luego se aplica un filtro para determinar si la traza se debe publicar para luego separa el stream en N streams, uno por cada manager o evento que queremos publicar.
A cada branch, se le aplica un Peek para establecer metricas, un map para transformar la entityTraceability en un evento final, nuevamente un peek para establecer metrica de cuantos eventos publicados ( Tener en cuenta que la funcion `FlatMapValues` puede generar mas de un evento, por eso la metrica se toma en este punto) y se termina con un sink al topico destino, el topico destino se obtiene de una funcion del manager utilizado.

```csharp

    StreamBuilder builder = new StreamBuilder();
    // Stream Configuration Here .......

    var branchs = builder
      .Stream<string, EntityTraceability, StringSerDes, SchemaAvroSerDes<EntityTraceability>>("entity-traceability")
      .Filter(ShoudPublish)
      .Branch(predicates.ToArray());

    int current = 0;

    foreach(var manager in _managers)
    {
      branchs[current]
        // Metrica para obtener el delay.
        .Peek((k, v) => MetricsRepository.DatetimeLastProducedTrace.SetToTimeUtc(v.stamp.ToUniversalTime()))
        // Convierte entityTraceability en 1 o mas eventos publicables
        .FlatMapValues(manager.Map)
        // Metrica de cantidad de eventos publicados
        .Peek((k, v) => MetricsRepository.EventsPublishedTotalCount.Inc())
        // obtiene el nombre del topico de salida, en principio solo retorna el nombre del topico
        // pero en los composite, tendremos que realizar alguna logica que nos de el topico para cada evento.
        .To<StringSerDes, SchemaAvroSerDes<ISpecificRecord>>(manager.ExtractTopicName);
      current++;
    }

    Topology t = builder.Build();

```

## Managers

Existira un manager por evento tal cual como venimos trabajando con el publicador MQ. La interfaz que debe implementar el manager se ha modificado para adaptar el mismo al stream.

```csharp
  public interface ITraceManager
  {
    // Transforma EntityTraceability en 1 o mas eventos en formato Avro.
    IEnumerable<ISpecificRecord> Map(EntityTraceability trace);
    // Estabece si este manager es el encargado de trabajar con la entityTraceability en cuestion.
    bool CanHandleTrace(string key, EntityTraceability trace);
    // Devuelve el nombre del topico de salida. En caso de que el manager genere mas de 1 evento de distinto tipo, esta funcion determina a que topico va cada evento.
    string ExtractTopicName(string key, ISpecificRecord record, IRecordContext context);
    // Nombre del topico.
    string TopicName { get; }
  }

```
