import socket
import random
import time

#���[�J���z�X�g
HOST = '127.0.0.1'
#�|�[�g�ԍ�
PORT = 50007

client = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

while True:
    # 0, 1, 2 �̃����_���Ȓl���擾
    a = random.randrange(3)
    result = str(a)
    print(a)
    
    client.sendto(result.encode('utf-8'), (HOST, PORT))
    time.sleep(2.0)