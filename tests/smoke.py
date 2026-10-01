import json,os,pathlib,subprocess,tempfile,time,urllib.request,urllib.error,socket
root=pathlib.Path(__file__).resolve().parents[1]
def call(method,path,data=None):
 req=urllib.request.Request(base+path,data=None if data is None else json.dumps(data).encode(),method=method,headers={'Content-Type':'application/json'})
 try:
  with urllib.request.urlopen(req) as r:return r.status,json.loads(r.read() or b'null')
 except urllib.error.HTTPError as e:return e.code,None
with tempfile.TemporaryDirectory() as d:
 with socket.socket() as s:s.bind(('127.0.0.1',0));port=s.getsockname()[1]
 base=f'http://127.0.0.1:{port}'
 env={**os.environ,'ConnectionStrings__Todos':f'Data Source={d}/test.db'}
 def start():
  p=subprocess.Popen(['dotnet','run','--no-build','--no-launch-profile','--urls',base],cwd=root,env=env,stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL)
  for _ in range(100):
   try:
    if call('GET','/todos')[0]==200:return p
   except OSError:pass
   time.sleep(.1)
  p.terminate();raise RuntimeError('Server did not start')
 p=start()
 try:
  assert call('GET','/todos')==(200,[])
  assert call('POST','/todos',{'name':'','dueDate':'2099-01-01'})[0]==400
  status,item=call('POST','/todos',{'name':'Demo','dueDate':'2099-01-01','isCompleted':False});assert status==201
  route='/todos/'+str(item['id'])
  assert call('PUT',route,{'name':'Updated','dueDate':'2099-01-01','isCompleted':True})[0]==200
  p.terminate();p.wait();p=start()
  assert call('GET',route)[1]['isCompleted'] is True
  assert call('DELETE',route)[0]==204
  assert call('GET',route)[0]==404
  print('PASS: CRUD, validation, missing task and persistence after restart')
 finally:p.terminate();p.wait()
