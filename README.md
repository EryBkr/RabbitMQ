# RabbitMQ yu Docker ile ayağa kaldırma işlemleri
Öncelikle RabbitMQ nun DockerHub içerisinde ki Management sürümünü kullanıyoruz.Bunun nedeni içerisinde monitoring ile birlikte gelmesi

`docker run -d -p 5672:5672 -p 15672:15672 --name rabbitmqcontainer rabbitmq:3-management`

İki adet port kullanma nedenimiz birisi monitoring için bir bağlantı oluştururken diğeri servis bağlantısı için kullanılacaktır.
Giriş bilgileri default olarak username:parola guest:guest şeklindedir.
Servis bağlantı adresimiz `amqp://guest:guest@localhost:5672` , monitoring adresimiz ise `localhost:15672` dir
