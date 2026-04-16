import os
import uuid
import threading
from flask import Flask, request, jsonify
import dashscope

# --- 【修改开始】 1. 引入 openai 库 ---
from openai import OpenAI

# --- 【修改结束】 ---

# --- 基本设置 ---
app = Flask(__name__)
UPLOAD_FOLDER = os.path.join(os.getcwd(), 'uploads')
if not os.path.exists(UPLOAD_FOLDER):
    os.makedirs(UPLOAD_FOLDER)
ALLOWED_EXTENSIONS = {'wav'}

# 全局字典，用来存放后台任务的结果
task_results = {}


def allowed_file(filename):
    return '.' in filename and filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS


# --- 【修改开始】 2. 函数重命名并替换为 DeepSeek 的调用逻辑 ---
def process_with_deepseek(task_id, raw_text, base_filename):
    """
    在后台调用 DeepSeek API，并将结果存入全局字典。
    同时，将两个文本都写入.txt文件。
    """
    global task_results
    processed_text = ""
    try:
        # 1. 初始化 DeepSeek 客户端
        # 注意：为了安全，建议未来将 API Key 存储在环境变量中
        client = OpenAI(
            api_key="xxxxxxx",
            base_url="https://api.deepseek.com"
        )

        # 2. 您的语言老师 Prompt (保持不变)
        system_prompt = 'xx'
        # 3. 发送请求
        print(f"[{task_id}] 后台开始调用 DeepSeek...")
        response = client.chat.completions.create(
            model="deepseek-chat",
            messages=[
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": raw_text}
            ],
            stream=False
        )

        processed_text = response.choices[0].message.content
        print(f"[{task_id}] DeepSeek 处理成功: {processed_text}")

    except Exception as e:
        print(f"[{task_id}] 调用 DeepSeek 时发生异常: {e}")
        processed_text = f"DeepSeek 处理失败: {e}"

    # 4. 将两个结果都保存到txt文件 (逻辑不变)
    try:
        text_filepath = os.path.join(UPLOAD_FOLDER, f"{base_filename}.txt")
        with open(text_filepath, 'w', encoding='utf-8') as text_file:
            text_file.write("--- ASR 原始结果 ---\n")
            text_file.write(raw_text + "\n\n")
            text_file.write("--- DeepSeek 润色结果 ---\n")
            text_file.write(processed_text + "\n")
        print(f"[{task_id}] 两个结果已保存到: {text_filepath}")
    except Exception as e:
        print(f"[{task_id}] 保存.txt日志时发生异常: {e}")

    # 5. 将最终结果存入全局字典，等待Unity来取 (逻辑不变)
    task_results[task_id] = processed_text


# --- 【修改结束】 ---


# --- API 路由 ---

@app.route('/upload', methods=['POST'])
def upload_file():
    if 'file' not in request.files: return jsonify({'status': 'error', 'message': '请求中没有文件部分'}), 400
    file = request.files['file']
    if file.filename == '': return jsonify({'status': 'error', 'message': '没有选择文件'}), 400

    if file and allowed_file(file.filename):
        # ... (文件保存逻辑不变) ...
        for filename in os.listdir(UPLOAD_FOLDER):
            if os.path.isfile(os.path.join(UPLOAD_FOLDER, filename)): os.remove(os.path.join(UPLOAD_FOLDER, filename))
        base_filename = "latest_recording"
        new_filename = f"{base_filename}.wav"
        local_file_path = os.path.join(UPLOAD_FOLDER, new_filename)
        file.save(local_file_path)

        # --- 阶段一：调用阿里云ASR (逻辑不变) ---
        raw_transcribed_text = ""
        try:
            messages = [{"role": "user", "content": [{"audio": f"file://{local_file_path}"}]}]
            response_asr = dashscope.MultiModalConversation.call(
                api_key="asda",
                model="qwen3-asr-flash",
                messages=messages,
                asr_options={"language": "ko", "enable_lid": True, "enable_itn": False}
            )
            if response_asr.status_code == 200:
                raw_transcribed_text = response_asr.output.choices[0].message.content[0]['text']
                print(f"ASR 原始识别结果: {raw_transcribed_text}")
            else:
                raise Exception(f"ASR识别失败: {response_asr.message}")
        except Exception as e:
            print(f"调用ASR服务时发生异常: {e}")
            return jsonify({'status': 'error', 'message': f'语音识别失败: {e}'}), 500

        # --- 阶段二：启动后台任务，并立即返回 ---
        task_id = str(uuid.uuid4())

        # --- 【修改开始】 3. 调用新的 DeepSeek 处理函数 ---
        thread = threading.Thread(target=process_with_deepseek, args=(task_id, raw_transcribed_text, base_filename))
        # --- 【修改结束】 ---

        thread.start()

        # 立即返回原始文本和任务ID (逻辑不变)
        return jsonify({
            'status': 'pending',
            'task_id': task_id,
            'raw_transcription': raw_transcribed_text
        }), 200

    else:
        return jsonify({'status': 'error', 'message': '只允许上传 WAV 文件'}), 415


# 新的API接口，用于查询结果 (逻辑不变)
@app.route('/result/<task_id>', methods=['GET'])
def get_result(task_id):
    global task_results
    if task_id in task_results:
        result = task_results.pop(task_id)
        return jsonify({'status': 'success', 'transcription': result})
    else:
        return jsonify({'status': 'processing'})


if __name__ == '__main__':
    app.run(debug=True, port=5001)